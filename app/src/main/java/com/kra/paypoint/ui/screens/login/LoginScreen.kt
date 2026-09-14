package com.kra.paypoint.ui.screens.login

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.foundation.Image
import androidx.compose.ui.res.painterResource
import com.kra.paypoint.R
import com.kra.paypoint.ui.screens.login.viewmodel.LoginUiState
import com.kra.paypoint.ui.screens.login.viewmodel.LoginViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginScreen(
    viewModel: LoginViewModel,
    onNavigateToDashboard: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()

    LaunchedEffect(uiState.isAuthenticated, uiState.deviceWarning) {
        // Hold on the login screen if setup produced a device-registration warning, so it's
        // actually seen instead of flashing past on the way to the dashboard.
        if (uiState.isAuthenticated && uiState.deviceWarning == null) {
            onNavigateToDashboard()
        }
    }

    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background),
        contentAlignment = Alignment.Center
    ) {
        if (uiState.checkingSetupState) {
            CircularProgressIndicator()
            return@Box
        }

        Card(
            modifier = Modifier
                .padding(24.dp)
                .fillMaxWidth(0.92f)
                .verticalScroll(rememberScrollState())
                .shadow(16.dp, RoundedCornerShape(16.dp)),
            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
            shape = RoundedCornerShape(16.dp)
        ) {
            Column(
                modifier = Modifier
                    .padding(32.dp)
                    .fillMaxWidth(),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Image(
                    painter = painterResource(id = R.drawable.etims_logo),
                    contentDescription = "eTIMS Logo",
                    modifier = Modifier
                        .height(80.dp)
                        .padding(bottom = 16.dp)
                )
                Text(
                    text = "eTIMS PayPoint",
                    style = MaterialTheme.typography.headlineMedium.copy(
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.primary
                    ),
                    modifier = Modifier.padding(bottom = 8.dp)
                )
                Text(
                    text = "Kenya Revenue Authority Point of Sale",
                    style = MaterialTheme.typography.bodyMedium.copy(
                        color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                    ),
                    modifier = Modifier.padding(bottom = 24.dp)
                )

                if (uiState.errorMessage != null) {
                    Text(
                        text = uiState.errorMessage ?: "",
                        color = MaterialTheme.colorScheme.error,
                        style = MaterialTheme.typography.bodySmall,
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(bottom = 16.dp)
                    )
                }

                if (uiState.isAuthenticated && uiState.deviceWarning != null) {
                    DeviceWarningNotice(uiState.deviceWarning!!, onContinue = onNavigateToDashboard)
                } else if (uiState.needsFirstRunSetup) {
                    FirstRunSetupForm(uiState, viewModel)
                } else {
                    SignInForm(uiState, viewModel)
                }
            }
        }
    }
}

@Composable
private fun DeviceWarningNotice(message: String, onContinue: () -> Unit) {
    Text(
        text = "Account created",
        style = MaterialTheme.typography.titleMedium.copy(fontWeight = FontWeight.SemiBold),
        modifier = Modifier.padding(bottom = 8.dp)
    )
    Text(
        text = message,
        style = MaterialTheme.typography.bodyMedium,
        color = MaterialTheme.colorScheme.error,
        modifier = Modifier.padding(bottom = 20.dp)
    )
    Button(
        onClick = onContinue,
        modifier = Modifier.fillMaxWidth().height(56.dp),
        shape = RoundedCornerShape(8.dp)
    ) {
        Text(text = "Continue to Dashboard", fontSize = 16.sp, fontWeight = FontWeight.SemiBold)
    }
}

@Composable
private fun SignInForm(uiState: LoginUiState, viewModel: LoginViewModel) {
    OutlinedTextField(
        value = uiState.username,
        onValueChange = viewModel::onUsernameChanged,
        label = { Text("Cashier User ID") },
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        singleLine = true,
        enabled = !uiState.isLoading
    )

    Spacer(modifier = Modifier.height(16.dp))

    OutlinedTextField(
        value = uiState.password,
        onValueChange = viewModel::onPasswordChanged,
        label = { Text("Password") },
        visualTransformation = PasswordVisualTransformation(),
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        singleLine = true,
        enabled = !uiState.isLoading
    )

    Spacer(modifier = Modifier.height(32.dp))

    Button(
        onClick = viewModel::login,
        enabled = !uiState.isLoading,
        modifier = Modifier
            .fillMaxWidth()
            .height(56.dp),
        shape = RoundedCornerShape(8.dp),
        colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
    ) {
        if (uiState.isLoading) {
            CircularProgressIndicator(
                color = MaterialTheme.colorScheme.onPrimary,
                modifier = Modifier.size(24.dp)
            )
        } else {
            Text(text = "Sign In", fontSize = 18.sp, fontWeight = FontWeight.SemiBold)
        }
    }
}

@Composable
private fun FirstRunSetupForm(uiState: LoginUiState, viewModel: LoginViewModel) {
    Text(
        text = "Device setup: no operator accounts exist yet on this device. " +
            "Create the first administrator account using your branch's real KRA details.",
        style = MaterialTheme.typography.bodySmall,
        color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f),
        modifier = Modifier.padding(bottom = 20.dp)
    )

    OutlinedTextField(
        value = uiState.fullName,
        onValueChange = viewModel::onFullNameChanged,
        label = { Text("Full Name") },
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.username,
        onValueChange = viewModel::onUsernameChanged,
        label = { Text("Admin Username") },
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.branchTin,
        onValueChange = viewModel::onBranchTinChanged,
        label = { Text("Branch TIN (KRA PIN)") },
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.branchId,
        onValueChange = viewModel::onBranchIdChanged,
        label = { Text("Branch ID (BHF ID)") },
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.deviceSerial,
        onValueChange = viewModel::onDeviceSerialChanged,
        label = { Text("Device Serial Number") },
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.password,
        onValueChange = viewModel::onPasswordChanged,
        label = { Text("Password") },
        visualTransformation = PasswordVisualTransformation(),
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )
    Spacer(modifier = Modifier.height(12.dp))
    OutlinedTextField(
        value = uiState.confirmPassword,
        onValueChange = viewModel::onConfirmPasswordChanged,
        label = { Text("Confirm Password") },
        visualTransformation = PasswordVisualTransformation(),
        modifier = Modifier.fillMaxWidth(),
        singleLine = true,
        enabled = !uiState.isLoading
    )

    Spacer(modifier = Modifier.height(24.dp))

    Button(
        onClick = viewModel::completeFirstRunSetup,
        enabled = !uiState.isLoading,
        modifier = Modifier.fillMaxWidth().height(56.dp),
        shape = RoundedCornerShape(8.dp),
        colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
    ) {
        if (uiState.isLoading) {
            CircularProgressIndicator(
                color = MaterialTheme.colorScheme.onPrimary,
                modifier = Modifier.size(24.dp)
            )
        } else {
            Text(text = "Create Admin Account", fontSize = 16.sp, fontWeight = FontWeight.SemiBold)
        }
    }
}
