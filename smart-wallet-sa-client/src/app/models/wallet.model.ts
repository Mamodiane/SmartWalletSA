export interface Wallet {
  id: number;
  balance: number;
  user: {
    id: number;
    fullName: string;
    email: string;
  };
}