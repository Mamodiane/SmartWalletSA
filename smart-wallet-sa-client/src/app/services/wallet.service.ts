import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Wallet } from '../models/wallet.model';

@Injectable({
  providedIn: 'root'
})
export class WalletService {
  private apiUrl = 'https://localhost:7121/api/wallet';

  constructor(private http: HttpClient) {}

  getWallet(): Observable<Wallet> {
    return this.http.get<Wallet>(this.apiUrl);
  }

  deposit(amount: number) {
    return this.http.post(`${this.apiUrl}/deposit`, { amount });
  }

  withdraw(amount: number) {
    return this.http.post(`${this.apiUrl}/withdraw`, { amount });
  }

  transfer(receiverEmail: string, amount: number) {
    return this.http.post(`${this.apiUrl}/transfer`, {
      receiverEmail,
      amount
    });
  }
}