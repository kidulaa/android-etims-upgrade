package com.kra.paypoint.ui.export

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import com.itextpdf.io.image.ImageDataFactory
import com.itextpdf.kernel.pdf.PdfDocument
import com.itextpdf.kernel.pdf.PdfWriter
import com.itextpdf.layout.Document
import com.itextpdf.layout.element.*
import com.itextpdf.layout.properties.HorizontalAlignment
import com.itextpdf.layout.properties.TextAlignment
import com.itextpdf.layout.properties.UnitValue
import com.itextpdf.layout.properties.VerticalAlignment
import com.itextpdf.layout.borders.Border
import com.kra.paypoint.R
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.domain.model.device.DeviceRegistration
import com.kra.paypoint.ui.components.QRCodeGenerator
import com.kra.paypoint.BuildConfig
import java.io.ByteArrayOutputStream
import java.io.File
import java.io.FileOutputStream
import java.text.SimpleDateFormat
import java.util.Locale

object PdfGenerator {

    /**
     * Generates an eTIMS invoice PDF for the given [transaction] and [items].
     */
    fun createInvoicePdf(
        context: Context,
        transaction: TransactionEntity,
        items: List<TransactionItemEntity>,
        registration: DeviceRegistration?
    ): File {
        val fileName = "Invoice_${transaction.invoiceNumber}.pdf"
        val file = File(context.cacheDir, fileName)

        FileOutputStream(file).use { outputStream ->
            val writer = PdfWriter(outputStream)
            val pdf = PdfDocument(writer)
            val document = Document(pdf)

            document.setMargins(30f, 30f, 30f, 30f)

            // 1. Header with Logo and Title
            addHeader(context, transaction, document)

            // 2. From/To Section
            addFromToSection(document, transaction, registration)

            // 3. Invoice Summary
            addInvoiceDetails(document, transaction, registration)

            // 4. Items Table
            addItemsTable(document, items)

            // 5. SCU (Fiscal) Information
            addFiscalInformation(document, transaction, registration)

            // 6. Tax Summary
            addTaxSummary(document, transaction)

            // 7. Footer
            addFooter(document)

            document.close()
        }
        return file
    }

    private fun addHeader(context: Context, transaction: TransactionEntity, document: Document) {
        val bitmap = BitmapFactory.decodeResource(context.resources, R.drawable.etims_logo)
        val stream = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream)
        val imageData = ImageDataFactory.create(stream.toByteArray())
        
        val logo = Image(imageData)
            .setHeight(60f)
            .setAutoScaleWidth(true)

        val table = Table(UnitValue.createPercentArray(floatArrayOf(1f, 3f)))
            .useAllAvailableWidth()
            .setMarginBottom(20f)

        table.addCell(
            Cell().add(logo)
                .setBorder(Border.NO_BORDER)
                .setVerticalAlignment(VerticalAlignment.MIDDLE)
        )

        val title = when (transaction.salesTypeCode) {
            "C" -> "CREDIT NOTE"
            else -> "FISCAL INVOICE"
        }

        table.addCell(
            Cell().add(Paragraph(title).setBold().setFontSize(24f))
                .setBorder(Border.NO_BORDER)
                .setVerticalAlignment(VerticalAlignment.MIDDLE)
                .setTextAlignment(TextAlignment.RIGHT)
        )

        document.add(table)
    }

    private fun addFromToSection(
        document: Document,
        transaction: TransactionEntity,
        registration: DeviceRegistration?
    ) {
        val table = Table(UnitValue.createPercentArray(floatArrayOf(1f, 1f)))
            .useAllAvailableWidth()
            .setMarginBottom(15f)

        // From (Taxpayer)
        val fromCell = Cell().add(
            Paragraph("FROM").setBold().setUnderline().setMarginBottom(5f)
        ).add(Paragraph(registration?.taxprNm ?: "eTIMS Merchant"))
            .add(Paragraph("PIN: ${registration?.tin ?: "-"}"))
            .add(Paragraph("Branch: ${registration?.bhfNm ?: registration?.branchId ?: "-"}"))
            .add(Paragraph("Address: ${registration?.locDesc ?: "-"}"))
            .setBorder(Border.NO_BORDER)

        // To (Customer)
        val toCell = Cell().add(
            Paragraph("TO").setBold().setUnderline().setMarginBottom(5f)
        ).add(Paragraph(transaction.customerName ?: "Walk-in Customer"))
            .add(Paragraph("PIN: ${transaction.customerTin ?: "-"}"))
            .setBorder(Border.NO_BORDER)

        table.addCell(fromCell)
        table.addCell(toCell)
        document.add(table)
    }

    private fun addInvoiceDetails(
        document: Document,
        transaction: TransactionEntity,
        registration: DeviceRegistration?
    ) {
        val table = Table(UnitValue.createPercentArray(floatArrayOf(1f, 1.5f)))
            .setWidth(UnitValue.createPointValue(250f))
            .setHorizontalAlignment(HorizontalAlignment.LEFT)
            .setMarginBottom(20f)

        table.addCell(Cell().add(Paragraph("INVOICE NO:").setBold()).setBorder(Border.NO_BORDER))
        table.addCell(Cell().add(Paragraph("${registration?.sdcId ?: "SDC"}/${transaction.invoiceNumber}")).setBorder(Border.NO_BORDER))
        
        table.addCell(Cell().add(Paragraph("DATE:").setBold()).setBorder(Border.NO_BORDER))
        table.addCell(Cell().add(Paragraph(formatDate(transaction.salesDate))).setBorder(Border.NO_BORDER))

        document.add(table)
    }

    private fun addItemsTable(document: Document, items: List<TransactionItemEntity>) {
        val table = Table(UnitValue.createPercentArray(floatArrayOf(3f, 1.5f, 1.5f, 1.5f, 1.5f)))
            .useAllAvailableWidth()
            .setMarginBottom(20f)

        val headers = listOf("Description", "Qty", "Unit Price", "Tax", "Total")
        headers.forEach { header ->
            table.addHeaderCell(Cell().add(Paragraph(header).setBold()).setBackgroundColor(com.itextpdf.kernel.colors.ColorConstants.LIGHT_GRAY))
        }

        items.forEach { item ->
            table.addCell(Cell().add(Paragraph(item.itemName)))
            table.addCell(Cell().add(Paragraph("%.2f".format(item.quantity))))
            table.addCell(Cell().add(Paragraph("%,.2f".format(item.unitPrice))))
            table.addCell(Cell().add(Paragraph(item.taxTypeCode)))
            table.addCell(Cell().add(Paragraph("%,.2f".format(item.totalAmount))).setTextAlignment(TextAlignment.RIGHT))
        }

        document.add(table)
    }

    private fun addFiscalInformation(
        document: Document,
        transaction: TransactionEntity,
        registration: DeviceRegistration?
    ) {
        document.add(Paragraph("FISCAL DATA").setBold().setMarginBottom(5f))

        val table = Table(UnitValue.createPercentArray(floatArrayOf(2f, 1f)))
            .useAllAvailableWidth()
            .setMarginBottom(15f)

        val infoTable = Table(UnitValue.createPercentArray(floatArrayOf(1f, 2f)))
            .useAllAvailableWidth()
            .setBorder(Border.NO_BORDER)

        infoTable.addCell(Cell().add(Paragraph("SDC ID:").setBold()).setBorder(Border.NO_BORDER))
        infoTable.addCell(Cell().add(Paragraph(registration?.sdcId ?: "-")).setBorder(Border.NO_BORDER))
        
        infoTable.addCell(Cell().add(Paragraph("MRC NO:").setBold()).setBorder(Border.NO_BORDER))
        infoTable.addCell(Cell().add(Paragraph(registration?.mrcNo ?: "-")).setBorder(Border.NO_BORDER))

        // Note: In real scenarios, internalData and rcptSign are part of the transaction payload
        // For this demo, we use the qrCodeData if available or placeholders
        val qrUrl = if (transaction.qrCodeData != null) {
            "${BuildConfig.RECEIPT_URL}common/link/etims/receipt/indexEtimsReceiptData?Data=${registration?.tin}${registration?.branchId}${transaction.qrCodeData}"
        } else {
            "https://etims.kra.go.ke"
        }
        
        table.addCell(Cell().add(infoTable).setBorder(Border.NO_BORDER))

        val qrBitmap = QRCodeGenerator.generate(qrUrl, 120)
        val stream = ByteArrayOutputStream()
        qrBitmap.compress(Bitmap.CompressFormat.PNG, 100, stream)
        val qrImage = Image(ImageDataFactory.create(stream.toByteArray()))
            .setHorizontalAlignment(HorizontalAlignment.RIGHT)

        table.addCell(Cell().add(qrImage).setBorder(Border.NO_BORDER).setVerticalAlignment(VerticalAlignment.BOTTOM))

        document.add(table)
    }

    private fun addTaxSummary(document: Document, transaction: TransactionEntity) {
        document.add(Paragraph("TAX SUMMARY").setBold().setMarginBottom(5f))

        val table = Table(UnitValue.createPercentArray(floatArrayOf(1f, 1.5f, 1.5f, 1.5f)))
            .setWidth(UnitValue.createPercentValue(80f))
            .setMarginBottom(20f)

        table.addHeaderCell(Cell().add(Paragraph("Label").setBold()))
        table.addHeaderCell(Cell().add(Paragraph("Taxable").setBold()))
        table.addHeaderCell(Cell().add(Paragraph("Tax").setBold()))
        table.addHeaderCell(Cell().add(Paragraph("Total").setBold()))

        fun addTaxRow(label: String, taxable: Double, tax: Double) {
            if (taxable == 0.0 && tax == 0.0) return
            table.addCell(Cell().add(Paragraph(label)))
            table.addCell(Cell().add(Paragraph("%,.2f".format(taxable))))
            table.addCell(Cell().add(Paragraph("%,.2f".format(tax))))
            table.addCell(Cell().add(Paragraph("%,.2f".format(taxable + tax))))
        }

        addTaxRow("A-16%", transaction.taxableAmountA, transaction.taxAmountA)
        addTaxRow("B-0%", transaction.taxableAmountB, transaction.taxAmountB)
        addTaxRow("C-Exempt", transaction.taxableAmountC, transaction.taxAmountC)
        addTaxRow("D-NonVAT", transaction.taxableAmountD, transaction.taxAmountD)
        addTaxRow("E-8%", transaction.taxableAmountE, transaction.taxAmountE)

        // Totals
        table.addCell(Cell().add(Paragraph("TOTALS").setBold()))
        table.addCell(Cell().add(Paragraph("%,.2f".format(transaction.totalTaxableAmount)).setBold()))
        table.addCell(Cell().add(Paragraph("%,.2f".format(transaction.totalTaxAmount)).setBold()))
        table.addCell(Cell().add(Paragraph("%,.2f".format(transaction.totalAmount)).setBold()))

        document.add(table)
    }

    private fun addFooter(document: Document) {
        document.add(
            Paragraph("Thank you for your business! Generated by eTIMS PayPoint.")
                .setTextAlignment(TextAlignment.CENTER)
                .setFontSize(10f)
                .setItalic()
                .setMarginTop(20f)
        )
    }

    private fun formatDate(dateStr: String): String {
        return try {
            // yyyyMMddHHmmss -> dd/MM/yyyy HH:mm
            val inputFormat = SimpleDateFormat("yyyyMMddHHmmss", Locale.getDefault())
            val outputFormat = SimpleDateFormat("dd/MM/yyyy HH:mm", Locale.getDefault())
            outputFormat.format(inputFormat.parse(dateStr)!!)
        } catch (e: Exception) {
            dateStr
        }
    }
}
