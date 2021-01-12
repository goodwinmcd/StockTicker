CREATE TABLE IF NOT EXISTS StockTickers (
    nasdaqSymbol TEXT PRIMARY KEY,
    Exchange TEXT,
    SecurityName Text
);

CREATE TABLE IF NOT EXISTS FoundMessage (
    id SERIAL PRIMARY KEY,
    source TEXT,
    subreddit TEXT,
    externalId TEXT,
    timePosted TimeStamp,
    message TEXT
);

CREATE TABLE IF NOT EXISTS stockTickersFoundMessage(
    foundMessageId INT NOT NULL,
    stockTickerId TEXT NOT NULL,
    PRIMARY KEY (foundMessageId, stockTickerId),
  FOREIGN KEY (foundMessageId)
      REFERENCES FoundMessage(id),
  FOREIGN KEY (stockTickerId)
      REFERENCES StockTickers (nasdaqSymbol)
);