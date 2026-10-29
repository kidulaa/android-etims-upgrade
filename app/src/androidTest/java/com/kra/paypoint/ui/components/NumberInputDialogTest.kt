package com.kra.paypoint.ui.components

import androidx.compose.ui.test.assertIsEnabled
import androidx.compose.ui.test.assertIsNotEnabled
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithText
import androidx.compose.ui.test.performClick
import androidx.compose.ui.test.performTextReplacement
import androidx.test.ext.junit.runners.AndroidJUnit4
import com.kra.paypoint.ui.theme.PayPointTheme
import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test
import org.junit.runner.RunWith

@RunWith(AndroidJUnit4::class)
class NumberInputDialogTest {

    @get:Rule
    val compose = createComposeRule()

    @Test
    fun confirmIsDisabledForInvalidInputAndReturnsParsedValue() {
        var confirmed: Double? = null
        compose.setContent {
            PayPointTheme {
                NumberInputDialog(title = "Quantity", label = "Qty", initialValue = 1.0, onConfirm = { confirmed = it }, onDismiss = {})
            }
        }

        compose.onNodeWithText("1").performTextReplacement("abc")
        compose.onNodeWithText("OK").assertIsNotEnabled()

        compose.onNodeWithText("abc").performTextReplacement("2.5")
        compose.onNodeWithText("OK").assertIsEnabled().performClick()
        assertEquals(2.5, confirmed!!, 0.0)
    }
}
