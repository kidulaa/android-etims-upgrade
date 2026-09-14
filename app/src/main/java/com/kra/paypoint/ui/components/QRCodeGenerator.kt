package com.kra.paypoint.ui.components

import android.graphics.Bitmap
import android.graphics.Color
import com.google.zxing.BarcodeFormat
import com.google.zxing.EncodeHintType
import com.google.zxing.qrcode.QRCodeWriter
import com.google.zxing.qrcode.decoder.ErrorCorrectionLevel

object QRCodeGenerator {
    /**
     * Generates a QR code bitmap for the given [content].
     */
    fun generate(content: String, size: Int = 250): Bitmap {
        val hints = hashMapOf<EncodeHintType, Any>().also {
            it[EncodeHintType.CHARACTER_SET] = "UTF-8"
            it[EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.H
            it[EncodeHintType.MARGIN] = 1
        }

        val writer = QRCodeWriter()
        val bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, size, size, hints)

        val width = bitMatrix.width
        val height = bitMatrix.height
        val bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.RGB_565)

        for (x in 0 until width) {
            for (y in 0 until height) {
                bitmap.setPixel(x, y, if (bitMatrix.get(x, y)) Color.BLACK else Color.WHITE)
            }
        }

        return bitmap
    }
}
