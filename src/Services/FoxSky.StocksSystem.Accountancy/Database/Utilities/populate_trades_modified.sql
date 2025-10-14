-- Create Trades table
CREATE TABLE IF NOT EXISTS Trades (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ticker TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    TradingAction INTEGER NOT NULL,
    DecisionTime TEXT NOT NULL
);

-- Insert 100 trade records
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 150, 145.23, 2, '2025-01-05 09:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 200, 298.50, 1, '2025-01-07 10:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 75, 2750.10, 2, '2025-01-10 14:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 50, 3200.75, 1, '2025-01-15 11:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 300, 850.40, 3, '2025-01-20 13:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 100, 305.67, 1, '2025-01-25 15:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 250, 165.89, 2, '2025-02-01 09:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 400, 140.12, 1, '2025-02-05 12:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 180, 155.34, 3, '2025-02-10 10:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 220, 175.56, 1, '2025-02-15 14:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 120, 148.90, 2, '2025-02-20 11:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 300, 295.45, 1, '2025-02-25 09:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 90, 2800.25, 3, '2025-03-01 13:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 60, 3150.80, 1, '2025-03-05 15:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 250, 860.12, 2, '2025-03-10 10:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 150, 310.23, 1, '2025-03-15 12:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 200, 170.67, 2, '2025-03-20 14:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 350, 145.89, 1, '2025-03-25 09:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 170, 160.45, 3, '2025-03-30 11:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 240, 180.34, 1, '2025-04-01 13:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 100, 150.12, 2, '2025-04-05 15:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 280, 300.78, 1, '2025-04-10 10:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 80, 2850.90, 2, '2025-04-15 12:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 70, 3100.45, 1, '2025-04-20 14:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 270, 870.56, 3, '2025-04-25 09:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 130, 315.78, 1, '2025-04-30 11:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 230, 175.23, 2, '2025-05-05 13:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 380, 150.67, 1, '2025-05-10 15:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 190, 165.89, 3, '2025-05-15 10:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 260, 185.12, 1, '2025-05-20 12:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 140, 152.34, 2, '2025-05-25 14:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 310, 305.45, 1, '2025-05-30 09:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 85, 2900.67, 2, '2025-06-01 11:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 65, 3050.23, 1, '2025-06-05 13:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 290, 880.90, 3, '2025-06-10 15:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 140, 320.12, 1, '2025-06-15 10:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 210, 180.56, 2, '2025-06-20 12:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 360, 155.34, 1, '2025-06-25 14:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 200, 170.78, 3, '2025-06-30 09:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 280, 190.45, 1, '2025-07-05 11:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 160, 155.67, 2, '2025-07-10 13:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 320, 310.89, 1, '2025-07-15 15:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 95, 2950.12, 2, '2025-07-20 10:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 55, 3000.78, 1, '2025-07-25 12:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 310, 890.34, 3, '2025-07-30 14:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 160, 325.56, 1, '2025-08-01 09:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 240, 185.23, 2, '2025-08-05 11:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 390, 160.90, 1, '2025-08-10 13:35:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 210, 175.12, 3, '2025-08-15 15:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 300, 195.67, 1, '2025-08-20 10:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 170, 158.90, 2, '2025-08-25 12:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 330, 315.34, 1, '2025-08-30 14:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 100, 3000.45, 2, '2025-09-01 09:55:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 60, 2950.12, 1, '2025-09-05 11:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 320, 900.78, 3, '2025-09-10 13:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 170, 330.89, 1, '2025-09-15 15:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 260, 190.45, 2, '2025-09-20 10:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 400, 165.23, 1, '2025-09-25 12:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 220, 180.56, 3, '2025-09-30 14:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 310, 200.12, 1, '2025-10-01 09:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 180, 162.34, 2, '2025-10-05 11:35:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 340, 320.67, 1, '2025-10-10 13:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 110, 3050.89, 2, '2025-01-12 15:05:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 65, 2900.34, 1, '2025-01-17 10:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 330, 910.23, 3, '2025-01-22 12:55:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 180, 335.45, 1, '2025-01-27 14:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 270, 195.78, 2, '2025-02-02 09:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 410, 170.12, 1, '2025-02-07 11:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 230, 185.90, 3, '2025-02-12 13:55:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 320, 205.34, 1, '2025-02-17 15:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 190, 165.67, 2, '2025-02-22 10:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 350, 325.12, 1, '2025-02-27 12:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 120, 3100.23, 2, '2025-03-04 14:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 70, 2850.78, 1, '2025-03-09 09:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 340, 920.56, 3, '2025-03-14 11:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 190, 340.89, 1, '2025-03-19 13:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 280, 200.12, 2, '2025-03-24 15:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 420, 175.45, 1, '2025-03-29 10:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 240, 190.34, 3, '2025-04-03 12:35:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 330, 210.67, 1, '2025-04-08 14:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 200, 168.90, 2, '2025-04-13 09:05:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 360, 330.45, 1, '2025-04-18 11:20:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 130, 3150.67, 2, '2025-04-23 13:35:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 75, 2800.12, 1, '2025-04-28 15:50:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 350, 930.89, 3, '2025-05-03 10:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 200, 345.23, 1, '2025-05-08 12:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 290, 205.56, 2, '2025-05-13 14:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 430, 180.78, 1, '2025-05-18 09:55:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('PG', 250, 195.67, 3, '2025-05-23 11:10:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('DIS', 340, 215.12, 1, '2025-05-28 13:25:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AAPL', 210, 172.34, 2, '2025-06-02 15:40:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('MSFT', 370, 335.78, 1, '2025-06-07 10:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('GOOGL', 140, 3200.90, 2, '2025-06-12 12:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('AMZN', 80, 2750.45, 1, '2025-06-17 14:30:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('TSLA', 360, 940.23, 3, '2025-06-22 09:45:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('NVDA', 210, 350.56, 1, '2025-06-27 11:00:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('JPM', 300, 210.89, 2, '2025-07-02 13:15:00');
INSERT INTO Trades (Ticker, Quantity, Price, TradingAction, DecisionTime) VALUES ('WMT', 440, 185.12, 1, '2025-07-07 15:30:00');