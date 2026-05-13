import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WalletService } from '../../services/wallet.service';
import { Wallet } from '../../models/wallet.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  wallet?: Wallet;

  errorMessage = '';
  successMessage = '';

  depositAmount = 0;
  withdrawAmount = 0;

  receiverEmail = '';
  transferAmount = 0;

  constructor(private walletService: WalletService) {}

  ngOnInit(): void {
    this.loadWallet();
  }

  loadWallet(): void {
    this.walletService.getWallet().subscribe({
      next: (response) => {
        this.wallet = response;
      },
      error: () => {
        this.errorMessage = 'Unable to load wallet.';
      }
    });
  }

  deposit(): void {
    this.walletService.deposit(this.depositAmount).subscribe({
      next: () => {
        this.successMessage = 'Deposit successful.';
        this.errorMessage = '';

        this.depositAmount = 0;

        this.loadWallet();
      },
      error: () => {
        this.errorMessage = 'Deposit failed.';
        this.successMessage = '';
      }
    });
  }

  withdraw(): void {
    this.walletService.withdraw(this.withdrawAmount).subscribe({
      next: () => {
        this.successMessage = 'Withdrawal successful.';
        this.errorMessage = '';

        this.withdrawAmount = 0;

        this.loadWallet();
      },
      error: (error) => {
        this.successMessage = '';

        this.errorMessage =
          error.error || 'Withdrawal failed.';
      }
    });
  }

  transfer(): void {
    this.walletService.transfer(
      this.receiverEmail,
      this.transferAmount
    ).subscribe({
      next: () => {
        this.successMessage = 'Transfer successful.';
        this.errorMessage = '';

        this.receiverEmail = '';
        this.transferAmount = 0;

        this.loadWallet();
      },
      error: (error) => {
        this.successMessage = '';

        this.errorMessage =
          error.error || 'Transfer failed.';
      }
    });
  }
}