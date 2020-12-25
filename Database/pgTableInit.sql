CREATE TABLE IF NOT EXISTS StockTickers (
    nasdaqSymbol TEXT PRIMARY KEY,
    Exchange TEXT,
    SecurityName Text
);

CREATE TABLE IF NOT EXISTS RedditMessage (
    id SERIAL PRIMARY KEY,
    source TEXT,
    subreddit TEXT,
    redditId TEXT,
    timePosted TimeStamp,
    message TEXT
);

CREATE TABLE IF NOT EXISTS stockTickersRedditMessage(
    redditMessageId INT NOT NULL,
    stockTickerId TEXT NOT NULL,
    PRIMARY KEY (redditMessageId, stockTickerId),
  FOREIGN KEY (redditMessageId)
      REFERENCES RedditMessage(id),
  FOREIGN KEY (stockTickerId)
      REFERENCES StockTickers (nasdaqSymbol)
);