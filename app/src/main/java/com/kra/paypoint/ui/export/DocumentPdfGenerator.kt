package com.kra.paypoint.ui.export

import android.content.Context
import com.itextpdf.kernel.colors.ColorConstants
import com.itextpdf.kernel.pdf.PdfDocument
import com.itextpdf.kernel.pdf.PdfWriter
import com.itextpdf.layout.Document
import com.itextpdf.layout.borders.Border
import com.itextpdf.layout.element.Cell
import com.itextpdf.layout.element.Paragraph
import com.itextpdf.layout.element.Table
import com.itextpdf.layout.properties.TextAlignment
import com.itextpdf.layout.properties.UnitValue
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import com.kra.paypoint.data.local.entity.DocumentType
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import com.kra.paypoint.domain.model.device.DeviceRegistration
import java.io.File
import java.io.FileOutputStream

/**
 * PDFs for the documents the KRA eTIMS mobile guide lets you "Download": quotations, proformas,
 * LPOs and recorded purchase invoices. Fiscal sales invoices keep their own layout in [PdfGenerator].
 */
object DocumentPdfGenerator {

    private data class Line(val name: String, val qty: Double, val price: Double, val discount: Double, val total: Double)

    private data class Model(
        val title: String,
        val number: String,
        val date: String,
        val dueDate: String?,
        val fromLabel: String,
        val fromLines: List<String>,
        val toLabel: String,
        val toLines: List<String>,
        val lines: List<Line>,
        val tax: Double,
        val total: Double,
        val note: String?,
        val footer: String
    )

    fun createForDocument(context: Context, doc: CommercialDocumentWithItems, registration: DeviceRegistration?): File {
        val d = doc.document
        val isLpo = d.docType == DocumentType.LPO
        val business = listOfNotNull(registration?.taxprNm ?: "eTIMS Merchant", registration?.tin?.let { "PIN: $it" }, registration?.locDesc)
        val party = listOfNotNull(d.partyName, d.partyTin?.let { "PIN: $it" }, d.partyPhone?.let { "Tel: $it" })
        return write(
            context,
            "${d.displayNumber}.pdf",
            Model(
                title = DocumentType.label(d.docType).uppercase(),
                number = d.displayNumber,
                date = ReceiptJournal.formatDate(d.docDate),
                dueDate = d.dueDate?.let(ReceiptJournal::formatDate),
                fromLabel = if (isLpo) "BUYER" else "FROM",
                fromLines = business,
                toLabel = if (isLpo) "SUPPLIER" else "TO",
                toLines = party,
                lines = doc.items.sortedBy { it.itemSequence }.map { Line(it.itemName, it.quantity, it.unitPrice, it.discountAmount, it.totalAmount) },
                tax = d.totalTaxAmount,
                total = d.totalAmount,
                note = d.note,
                footer = "This is not a tax invoice. A KRA eTIMS invoice is issued on payment / delivery."
            )
        )
    }

    fun createForPurchase(context: Context, p: PurchaseWithItems, registration: DeviceRegistration?): File {
        val pu = p.purchase
        return write(
            context,
            "Purchase_${pu.purchaseNumber}.pdf",
            Model(
                title = "PURCHASE INVOICE",
                number = "PUR${pu.purchaseNumber.toString().padStart(6, '0')} (supplier inv ${pu.supplierInvoiceNumber})",
                date = ReceiptJournal.formatDate(pu.purchaseDate),
                dueDate = null,
                fromLabel = "SUPPLIER",
                fromLines = listOf(pu.supplierName, "PIN: ${pu.supplierTin}"),
                toLabel = "BUYER",
                toLines = listOfNotNull(registration?.taxprNm ?: "eTIMS Merchant", registration?.tin?.let { "PIN: $it" }),
                lines = p.items.sortedBy { it.itemSequence }.map { Line(it.itemName, it.quantity, it.unitPrice, it.discountAmount, it.totalAmount) },
                tax = pu.totalTaxAmount,
                total = pu.totalAmount,
                note = pu.note,
                footer = "Recorded in eTIMS PayPoint • KRA status: ${pu.syncStatus}"
            )
        )
    }

    private fun write(context: Context, fileName: String, m: Model): File {
        val file = File(context.cacheDir, fileName)
        FileOutputStream(file).use { out ->
            val document = Document(PdfDocument(PdfWriter(out)))
            document.setMargins(30f, 30f, 30f, 30f)

            document.add(Paragraph(m.title).setBold().setFontSize(22f).setTextAlignment(TextAlignment.RIGHT))
            document.add(Paragraph("No. ${m.number}").setTextAlignment(TextAlignment.RIGHT))
            document.add(Paragraph("Date: ${m.date}" + (m.dueDate?.let { "    Due: $it" } ?: "")).setTextAlignment(TextAlignment.RIGHT).setMarginBottom(15f))

            val parties = Table(UnitValue.createPercentArray(floatArrayOf(1f, 1f))).useAllAvailableWidth().setMarginBottom(15f)
            listOf(m.fromLabel to m.fromLines, m.toLabel to m.toLines).forEach { (label, lines) ->
                val cell = Cell().setBorder(Border.NO_BORDER).add(Paragraph(label).setBold().setUnderline())
                lines.forEach { cell.add(Paragraph(it)) }
                parties.addCell(cell)
            }
            document.add(parties)

            val table = Table(UnitValue.createPercentArray(floatArrayOf(3.5f, 1f, 1.5f, 1.5f, 1.7f))).useAllAvailableWidth().setMarginBottom(12f)
            listOf("Item", "Qty", "Price", "Discount", "Total").forEach {
                table.addHeaderCell(Cell().add(Paragraph(it).setBold()).setBackgroundColor(ColorConstants.LIGHT_GRAY))
            }
            m.lines.forEach { l ->
                table.addCell(Cell().add(Paragraph(l.name)))
                table.addCell(Cell().add(Paragraph(if (l.qty % 1.0 == 0.0) l.qty.toLong().toString() else "%.2f".format(l.qty))))
                table.addCell(Cell().add(Paragraph("%,.2f".format(l.price))))
                table.addCell(Cell().add(Paragraph(if (l.discount > 0) "%,.2f".format(l.discount) else "-")))
                table.addCell(Cell().add(Paragraph("%,.2f".format(l.total))).setTextAlignment(TextAlignment.RIGHT))
            }
            document.add(table)

            val subtotal = m.lines.sumOf { it.price * it.qty }
            val discount = m.lines.sumOf { it.discount }
            listOfNotNull(
                "Subtotal: KES %,.2f".format(subtotal),
                discount.takeIf { it > 0 }?.let { "Discount: KES %,.2f".format(it) },
                "VAT included: KES %,.2f".format(m.tax)
            ).forEach { document.add(Paragraph(it).setTextAlignment(TextAlignment.RIGHT)) }
            document.add(Paragraph("TOTAL: KES %,.2f".format(m.total)).setBold().setFontSize(14f).setTextAlignment(TextAlignment.RIGHT))

            m.note?.let { document.add(Paragraph("Note: $it").setMarginTop(12f)) }
            document.add(Paragraph(m.footer).setItalic().setFontSize(9f).setTextAlignment(TextAlignment.CENTER).setMarginTop(24f))
            document.close()
        }
        return file
    }
}
