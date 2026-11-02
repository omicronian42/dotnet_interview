# Order Revenue - technical exercise

Thanks for taking the time. This exercise is a small version of the work you would do with us: a small .NET API, built around a business question that is not fully specified.

## Read this first: time and expectations

- **You have 2 days from when you receive this. Please do not spend more than 6 hours in total.** We are not asking for more, and we will not reward it.
- The scope is small on purpose. We would much rather see good judgement on a simple, working solution than a gold-plated one. When your time is up, stop and tell us what is left.
- **If you are short on time, protect this order:** (1) the revenue summary, with numbers you can defend, (2) the concurrent-edit handling, (3) `DECISIONS.md`. Tests are optional.
- **Use any tools you like, including AI assistants.** Be aware that the brief is intentionally open, so an assistant cannot make the calls for you. We want *your* decisions and the reasons for them. You are responsible for every line you submit, and in the review we will ask you to explain it and change it live.
- **The review is the main event.** What you can explain and defend matters more than how much you built (see "The review" below).

## Scenario

A mid-size B2B distributor of electrical and industrial supplies wants an internal **Order Revenue** tool for its finance team. Today Finance builds the monthly revenue report by hand in Excel; their spreadsheet currently shows **October 2025 revenue of ₹1,20,00,000**.

- Finance will use the monthly numbers for **management reporting**, so they must be defensible.
- In production there will be about **20 million orders a year** and about **50 people using the tool daily**.
- Treat the data as you would data from a real client.

## What is in this folder

| Path | What it is |
|---|---|
| `data/` | Four CSV extracts (customers, products, orders, order lines) covering April 2025 to March 2026, extracted on 2 April 2026 |
| `src/OrderRevenue.Api/` | .NET 8 Web API (controllers, services, EF Core + SQLite, CSV seed loader). `GET /api/orders` with paging already works |
| `src/OrderRevenue.Api/wwwroot/` | A small AngularJS orders list, served by the API. It has one display bug for you to fix (see below). Its search box and sort headers are not wired up and are out of scope. The summary page prints whatever JSON your endpoint returns |
| `tests/OrderRevenue.Tests/` | xUnit project with one tiny example test |
| `DECISIONS.md` | Template for your decisions. **Please fill this in.** |

## What to build

1. **Fix the orders page.** When you first run the app, the orders list does not display properly. The cause is in the front-end code. Fix it and tell us in `DECISIONS.md` what was wrong.
2. **`GET /api/reports/revenue-summary`**: monthly revenue over the data's date range, and the top 5 customers by revenue. The response shape and the definition of "revenue" are yours to decide.
3. **`PUT /api/orders/{id}`**: Finance users can change an order's status and discount. Two of them may well have the same order open at the same time. Decide how the API behaves and what the caller gets back. Swagger is enough to exercise it.
4. **`DECISIONS.md`**: your choices and the reasons for them.

You may change the structure of the starter if you have a good reason. Tell us why.

## Running the starter

Prerequisites: .NET 8 SDK.

```bash
cd src/OrderRevenue.Api
dotnet run
```

- App: http://localhost:5080
- API explorer (Swagger): http://localhost:5080/swagger
- The first start creates `orders.db` and loads the CSVs (a few seconds). Delete `orders.db` to re-seed.
- Tests: `dotnet test` from this folder.

## Notes on the starter

- Storage is SQLite with `EnsureCreated` (no migrations). If you change the model, delete `orders.db` and restart.
- Money columns are stored as `REAL` because the SQLite provider cannot sort or sum `decimal`. That is a convenience for this exercise, not a recommendation. If you think it matters, say so in `DECISIONS.md`.
- The seed loader skips rows whose primary key was already loaded (it logs how many), trims the status text and parses it into an enum. Otherwise it loads the CSVs as they are.
- All timestamps are UTC. `ShippedAtUtc` can be empty.
- AngularJS 1.8.3 is vendored in `wwwroot/lib`. It is past end of life, but it is the stack this exercise targets.

## Data dictionary

All amounts are in INR, excluding GST. All timestamps are ISO-8601 in UTC.

| File | Column | Meaning |
|---|---|---|
| `customers.csv` | `customer_id`, `name` | Customer record |
| | `gstin` | The customer's GST registration number |
| | `city`, `state`, `region` | Location; region is North / South / East / West |
| | `created_at` | Date the record was created |
| `products.csv` | `product_id`, `sku`, `name`, `category` | Product |
| | `list_price` | **Current** list price |
| `orders.csv` | `order_id`, `customer_id` | Order header |
| | `order_ts_utc` | When the order was placed |
| | `ship_ts_utc` | When it shipped (empty if not shipped) |
| | `status` | Placed, Shipped, Delivered, Cancelled or Returned, as recorded by the order system |
| | `discount_amount` | Order-level discount |
| | `order_total` | Total recorded on the order header |
| `order_lines.csv` | `line_id`, `order_id`, `product_id` | Order line |
| | `quantity` | Units ordered |
| | `unit_price` | Price per unit at the time of the order |
| | `returned_qty` | Units the customer sent back (0 if none) |

## What to hand back

1. The code, as a zip or a link to a private repo. Please `git init` and commit as you go; normal-sized commits are fine, we will glance at the history.
2. `DECISIONS.md`, filled in.
3. Any change to the run steps above.

## The review (45-60 minutes, screen share)

Be ready to run the app on your own machine. No slides. Roughly:

1. **Walkthrough (~15 min).** Show it running, then show us the code.
2. **Decisions (~15 min).** We will go through `DECISIONS.md` and ask why, and what would change your mind.
3. **Confidence in the numbers (~5-10 min).** How you checked that your output is right, and anything you would flag to the business or to us.
4. **A small change to your own code, live (~15-20 min).** AI tools are allowed.

## What we look at

- The judgement you apply to open questions, and how clearly you explain it
- Numbers you can defend, and how you know they are right
- A sound core: the API, the concurrent-edit handling, a revenue endpoint that holds up at the stated scale
- Honesty about what is unfinished
- How comfortably you navigate and change your own code

We do not mark on UI styling, the number of tests, or folder-structure purity.

Good luck, and please do not stay up late over this.
