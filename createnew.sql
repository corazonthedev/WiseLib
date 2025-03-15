CREATE TABLE "book_transactions" (
	"id"	INTEGER,
	"member_id"	TEXT NOT NULL,
	"ISBN"	TEXT NOT NULL,
	"borrow_date"	TEXT NOT NULL,
	"return_date"	TEXT NOT NULL,
	"transaction_type"	INTEGER NOT NULL,
	PRIMARY KEY("id")
);
CREATE TABLE "books" (
	"id"	INTEGER NOT NULL UNIQUE,
	"ISBN"	TEXT NOT NULL UNIQUE,
	"title"	TEXT NOT NULL,
	"author"	TEXT NOT NULL,
	"category"	TEXT,
	"page_count"	INTEGER NOT NULL,
	"year_of_publication"	TEXT NOT NULL,
	"publisher"	TEXT NOT NULL,
	"image_s"	TEXT,
	"image_m"	TEXT,
	"image_l"	TEXT,
	"taken"	INTEGER NOT NULL,
	"missing"	INTEGER NOT NULL,
	"has_problem"	INTEGER NOT NULL,
	"date_added"	TEXT,
	PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE TABLE "categories" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"category"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE "log_transactions" (
	"transaction_id"	INTEGER NOT NULL UNIQUE,
	"transaction_no"	TEXT NOT NULL,
	"transaction_date"	TEXT NOT NULL,
	"member_id"	TEXT,
	"book_isbn"	TEXT,
	PRIMARY KEY("transaction_id" AUTOINCREMENT)
);
CREATE TABLE "members" (
	"member_id"	INTEGER NOT NULL UNIQUE,
	"member_name"	TEXT NOT NULL,
	"member_surname"	TEXT NOT NULL,
	"borrow_status"	TEXT NOT NULL,
	"borrowed_book_ISBN"	TEXT,
	"borrow_date"	TEXT,
	"return_date"	TEXT,
	"join_date"	TEXT NOT NULL,
	"credit_point"	INTEGER NOT NULL,
	"banned"	INTEGER NOT NULL,
	"ban_date"	TEXT,
	PRIMARY KEY("member_id" AUTOINCREMENT)
);
CREATE TABLE "reports" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"TYPE"	TEXT NOT NULL,
	"ISBN"	TEXT,
	"NO"	TEXT,
	"TITLE"	TEXT NOT NULL,
	"DESC"	TEXT NOT NULL,
	"SOLVED"	TEXT NOT NULL,
	"CREATE_DATE"	TEXT NOT NULL,
	"SOLVE_DATE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
PRAGMA journal_mode=WAL;
