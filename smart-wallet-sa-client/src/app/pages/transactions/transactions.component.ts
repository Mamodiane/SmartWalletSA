import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.css'
})
export class TransactionsComponent implements OnInit {
  transactions: Transaction[] = [];

  selectedType = 'All';
  errorMessage = '';

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.transactionService.getTransactions().subscribe({
      next: (response) => {
        this.transactions = response;
      },
      error: () => {
        this.errorMessage = 'Unable to load transactions.';
      }
    });
  }

  get filteredTransactions(): Transaction[] {
    if (this.selectedType === 'All') {
      return this.transactions;
    }

    return this.transactions.filter(
      transaction => transaction.type === this.selectedType
    );
  }

  get totalDeposits(): number {
    return this.transactions
      .filter(t => t.type === 'Deposit')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  get totalWithdrawals(): number {
    return this.transactions
      .filter(t => t.type === 'Withdraw')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  get totalTransfers(): number {
    return this.transactions
      .filter(
        t =>
          t.type === 'Transfer Out' ||
          t.type === 'Transfer In'
      )
      .reduce((sum, t) => sum + t.amount, 0);
  }

  get transactionCount(): number {
    return this.transactions.length;
  }
}