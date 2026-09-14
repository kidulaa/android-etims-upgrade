package com.kra.paypoint.ui.screens.onboarding

import androidx.compose.foundation.ExperimentalFoundationApi
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.pager.HorizontalPager
import androidx.compose.foundation.pager.rememberPagerState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Description
import androidx.compose.material.icons.filled.PointOfSale
import androidx.compose.material.icons.filled.Receipt
import androidx.compose.material.icons.filled.RequestQuote
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.ui.screens.onboarding.viewmodel.OnboardingViewModel
import kotlinx.coroutines.launch

data class OnboardingPage(
    val title: String,
    val description: String,
    val icon: ImageVector
)

val onboardingPages = listOf(
    OnboardingPage(
        title = "Manage Documents",
        description = "Easily manage your taxpayer documents and business records in one place.",
        icon = Icons.Filled.Description
    ),
    OnboardingPage(
        title = "Tax Filing",
        description = "Simplify your tax calculations and ensure compliance with KRA regulations.",
        icon = Icons.Filled.RequestQuote
    ),
    OnboardingPage(
        title = "Invoices & Receipts",
        description = "Generate professional eTIMS invoices and receipts for your customers instantly.",
        icon = Icons.Filled.Receipt
    ),
    OnboardingPage(
        title = "Point of Sale",
        description = "Seamless point of sale experience tailored for your retail needs.",
        icon = Icons.Filled.PointOfSale
    )
)

@OptIn(ExperimentalFoundationApi::class)
@Composable
fun OnboardingScreen(
    viewModel: OnboardingViewModel,
    onNavigateToLogin: () -> Unit
) {
    val pagerState = rememberPagerState(pageCount = { onboardingPages.size })
    val coroutineScope = rememberCoroutineScope()

    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
    ) {
        Column(
            modifier = Modifier.fillMaxSize()
        ) {
            HorizontalPager(
                state = pagerState,
                modifier = Modifier.weight(1f)
            ) { page ->
                OnboardingPageContent(page = onboardingPages[page])
            }

            BottomSection(
                pagerState = pagerState,
                onNextClick = {
                    if (pagerState.currentPage < onboardingPages.size - 1) {
                        coroutineScope.launch {
                            pagerState.animateScrollToPage(pagerState.currentPage + 1)
                        }
                    } else {
                        viewModel.completeOnboarding()
                        onNavigateToLogin()
                    }
                },
                onSkipClick = {
                    viewModel.completeOnboarding()
                    onNavigateToLogin()
                }
            )
        }
    }
}

@Composable
fun OnboardingPageContent(page: OnboardingPage) {
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(32.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        // Placeholder for user images. Using Material Icons for now.
        Box(
            modifier = Modifier
                .size(200.dp)
                .clip(RoundedCornerShape(16.dp))
                .background(MaterialTheme.colorScheme.primaryContainer),
            contentAlignment = Alignment.Center
        ) {
            Icon(
                imageVector = page.icon,
                contentDescription = null,
                modifier = Modifier.size(100.dp),
                tint = MaterialTheme.colorScheme.onPrimaryContainer
            )
        }

        Spacer(modifier = Modifier.height(48.dp))

        Text(
            text = page.title,
            style = MaterialTheme.typography.headlineMedium.copy(
                fontWeight = FontWeight.Bold,
                color = MaterialTheme.colorScheme.onBackground
            ),
            textAlign = TextAlign.Center
        )

        Spacer(modifier = Modifier.height(16.dp))

        Text(
            text = page.description,
            style = MaterialTheme.typography.bodyLarge.copy(
                color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f)
            ),
            textAlign = TextAlign.Center
        )
    }
}

@OptIn(ExperimentalFoundationApi::class)
@Composable
fun BottomSection(
    pagerState: androidx.compose.foundation.pager.PagerState,
    onNextClick: () -> Unit,
    onSkipClick: () -> Unit
) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(24.dp),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        TextButton(
            onClick = onSkipClick,
            modifier = Modifier.weight(1f)
        ) {
            Text(
                text = "Skip",
                color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f),
                fontSize = 16.sp
            )
        }

        Row(
            modifier = Modifier.weight(1f),
            horizontalArrangement = Arrangement.Center
        ) {
            repeat(onboardingPages.size) { iteration ->
                val color = if (pagerState.currentPage == iteration)
                    MaterialTheme.colorScheme.primary
                else
                    MaterialTheme.colorScheme.onSurface.copy(alpha = 0.2f)

                Box(
                    modifier = Modifier
                        .padding(4.dp)
                        .clip(CircleShape)
                        .background(color)
                        .size(10.dp)
                )
            }
        }

        Button(
            onClick = onNextClick,
            modifier = Modifier.weight(1f),
            shape = RoundedCornerShape(8.dp),
            colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
        ) {
            Text(
                text = if (pagerState.currentPage == onboardingPages.size - 1) "Get Started" else "Next",
                fontSize = 16.sp,
                fontWeight = FontWeight.SemiBold
            )
        }
    }
}
