package com.kra.paypoint.hardware.printer

import android.content.Context
import android.graphics.Paint
import android.print.pdf.PrintedPdfDocument
import android.os.Bundle
import android.os.CancellationSignal
import android.os.ParcelFileDescriptor
import android.print.PageRange
import android.print.PrintAttributes
import android.print.PrintDocumentAdapter
import android.print.PrintDocumentInfo
import android.print.PrintManager
import java.io.FileOutputStream
import java.io.IOException

/**
 * Renders a plain-text A4 invoice into a paginated PDF and hands it to Android's print
 * framework, so it can go to any registered print service (a network/USB printer, "Save as
 * PDF", a cloud print provider) — replacing the legacy Windows-only GDI+ pipeline the
 * EBM2x.Tablet head used, which only worked with printers wired through Windows drivers.
 */
object A4InvoicePrintAdapter {

    fun print(context: Context, invoiceData: String, isReprint: Boolean) {
        val printManager = context.getSystemService(Context.PRINT_SERVICE) as PrintManager
        val jobName = if (isReprint) "eTIMS Invoice (Copy)" else "eTIMS Invoice"
        val attributes = PrintAttributes.Builder()
            .setMediaSize(PrintAttributes.MediaSize.ISO_A4)
            .setResolution(PrintAttributes.Resolution("pdf", "PDF", 300, 300))
            .setMinMargins(PrintAttributes.Margins.NO_MARGINS)
            .build()

        printManager.print(jobName, TextPrintDocumentAdapter(context, invoiceData, isReprint), attributes)
    }

    private class TextPrintDocumentAdapter(
        private val context: Context,
        private val text: String,
        private val isReprint: Boolean
    ) : PrintDocumentAdapter() {

        private var pdfDocument: PrintedPdfDocument? = null

        override fun onLayout(
            oldAttributes: PrintAttributes?,
            newAttributes: PrintAttributes,
            cancellationSignal: CancellationSignal?,
            callback: LayoutResultCallback,
            extras: Bundle?
        ) {
            pdfDocument = PrintedPdfDocument(context, newAttributes)
            if (cancellationSignal?.isCanceled == true) {
                callback.onLayoutCancelled()
                return
            }
            val info = PrintDocumentInfo.Builder("etims_invoice.pdf")
                .setContentType(PrintDocumentInfo.CONTENT_TYPE_DOCUMENT)
                .build()
            callback.onLayoutFinished(info, oldAttributes != newAttributes)
        }

        override fun onWrite(
            pages: Array<out PageRange>,
            destination: ParcelFileDescriptor,
            cancellationSignal: CancellationSignal?,
            callback: WriteResultCallback
        ) {
            val document = pdfDocument ?: run {
                callback.onWriteFailed("Document not laid out")
                return
            }
            try {
                val page = document.startPage(0)
                val canvas = page.canvas
                val paint = Paint().apply { textSize = 11f }
                var y = 24f
                val lineHeight = paint.fontSpacing

                if (isReprint) {
                    val bold = Paint(paint).apply { isFakeBoldText = true }
                    canvas.drawText("*** REPRINT / COPY ***", 24f, y, bold)
                    y += lineHeight * 1.5f
                }
                for (line in text.split("\n")) {
                    if (y > canvas.height - 24f) break // single-page MVP; multi-page pagination is future work
                    canvas.drawText(line, 24f, y, paint)
                    y += lineHeight
                }
                document.finishPage(page)

                FileOutputStream(destination.fileDescriptor).use { out ->
                    document.writeTo(out)
                }
                callback.onWriteFinished(arrayOf(PageRange.ALL_PAGES))
            } catch (e: IOException) {
                callback.onWriteFailed(e.message)
            } finally {
                document.close()
                pdfDocument = null
            }
        }
    }
}
