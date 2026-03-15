# CS690 Final Project

## Gift Planner

Gift Planner is a simple .NET console application for managing gift ideas, purchases, and important dates for people throughout the year.

## Version
v2.0.0

## Features
- Add and manage people
- Delete people
- Store gift ideas for each person
- Remove gift ideas
- Automatically track purchased gifts
- Record purchases
- Track total spending per person
- Add important dates (birthdays, anniversaries, etc.)
- View upcoming important dates
- Receive reminders for important dates within 7 days
- Data automatically persists between sessions

## Download and Run

The recommended way to run the application is to download the compiled version from the **Releases** section.

1. Go to the repository:
   
   https://github.com/ryanmhamelbsu/CS690-Final-Project

2. Click **Releases**

3. Download the latest release package:

```
GiftPlanner.zip
```

4. Extract the ZIP file.

5. In the extracted folder's address bar type cmd and hit enter.

6. Run the following command first to activate emoji's:

```bash
chcp 65001
```

7. In CMD run:

```bash
dotnet GiftPlanner.dll
```

## Running from Source (Optional)

If you prefer to run the project from the source code:

```bash
dotnet run --project GiftPlanner
```

## Running Tests

Automated tests are included using **xUnit**.

To run the tests from the repository root:

```bash
dotnet test
```

These tests verify core application behavior such as:

- Adding people
- Deleting people
- Removing gift ideas

## Documentation

Full documentation is available in the project Wiki, including:

- User Documentation
- Development Documentation
- Deployment Documentation
