# Frontend-Backend Setup & Troubleshooting Checklist

## 🚀 Quick Start

### Prerequisites
- Visual Studio (for backend) or `dotnet` CLI
- Node.js & npm (for frontend)
- MySQL running with database `library management system`
- Database user: `library_dev` / password: `password123`

### Step 1: Start Backend
```bash
cd LMSBackend
dotnet run --launch-profile https
```
Expected output:
```
Now listening on: https://localhost:7076
Now listening on: http://localhost:5266
```

### Step 2: Verify Backend Health
Open in browser: `https://localhost:7076/swagger`
Should see Swagger UI with all endpoints.

### Step 3: Test API Endpoint
Open in browser: `https://localhost:7076/borrow-request/get-pending`
Should return JSON array (even if empty `[]`).

### Step 4: Start Frontend
```bash
cd LMSFrontend
npm install  # First time only
ng serve
# or
npm start
```
Expected output:
```
✔ Compiled successfully
Server started at http://localhost:4200
```

### Step 5: Test in Browser
1. Open: `http://localhost:4200`
2. Navigate to Pending Requests
3. Should load list of requests

---

## 🔧 Troubleshooting: "Failed to load pending requests"

### Checklist 1: Is Backend Running?

- [ ] Open terminal/command prompt
- [ ] Run: `netstat -ano | findstr 7076` (Windows) or `lsof -i :7076` (Mac/Linux)
- [ ] If nothing shows, backend is NOT running
- [ ] Start backend: `cd LMSBackend && dotnet run --launch-profile https`

### Checklist 2: Can You Access Backend Swagger?

- [ ] Open: `https://localhost:7076/swagger`
- [ ] If page loads: Backend is responding ✅
- [ ] If error/blank: Backend has issue
- [ ] Check backend console for errors

### Checklist 3: Can You Call API Directly?

- [ ] Open: `https://localhost:7076/borrow-request/get-pending`
- [ ] If returns `[]` or JSON array: API works ✅
- [ ] If error page: Check backend console
- [ ] If "Cannot GET": Endpoint routing issue

### Checklist 4: Check Browser Console

- [ ] Press F12 → Console tab
- [ ] Refresh page
- [ ] Look for any red error messages
- [ ] Common errors:
  - `CORS error` → Backend CORS misconfigured
  - `ERR_CONNECTION_REFUSED` → Backend not running
  - `net::ERR_CERT_AUTHORITY_INVALID` → HTTPS certificate issue

### Checklist 5: Check Network Tab

- [ ] Press F12 → Network tab
- [ ] Refresh page
- [ ] Look for request starting with `get-pending`
- [ ] Click on it, check:
  - **Status:** Should be 200 (not 404, 500, etc.)
  - **Response:** Should show JSON array
  - **Headers:** Check `Access-Control-Allow-Origin`

### Checklist 6: Database Connected?

- [ ] Open MySQL terminal:
  ```bash
  mysql -h localhost -u library_dev -p
  # Password: password123
  ```
- [ ] Run:
  ```sql
  USE `library management system`;
  SHOW TABLES;
  SELECT COUNT(*) FROM BorrowRequests;
  ```
- [ ] If tables exist: Database OK ✅
- [ ] If error: Database not set up

### Checklist 7: Both Running on Right Ports?

| Component | Expected | Actual |
|-----------|----------|--------|
| Backend HTTPS | `https://localhost:7076` | ? |
| Backend HTTP | `http://localhost:5266` | ? |
| Frontend | `http://localhost:4200` | ? |
| Database | MySQL on `localhost:3306` | ? |

Use: `netstat -ano` (Windows) to check active ports.

---

## 🔍 Common Issues & Solutions

### Issue 1: CORS Error
**Error Message:** "Access to XMLHttpRequest blocked by CORS policy"

**Fix:**
1. Edit `LMS Backend/Program.cs`
2. Find CORS configuration (line 35-42)
3. Add frontend URL:
```csharp
policy.WithOrigins(
    "https://localhost:4200",
    "http://localhost:4200"
)
```
4. Restart backend (Ctrl+C, then run again)

### Issue 2: ERR_CONNECTION_REFUSED
**Error Message:** "Failed to load pending requests"

**Fix:**
1. Backend not running
2. Start backend: `dotnet run --launch-profile https`
3. Verify Swagger loads: `https://localhost:7076/swagger`

### Issue 3: 404 Not Found
**Error Message:** "Cannot GET /borrow-request/get-pending"

**Fix:**
1. Check controller is registered in `Program.cs`
2. Verify endpoint exists in `BorrowRequestController.cs`
3. Restart backend

### Issue 4: 500 Internal Server Error
**Error Message:** "An error occurred processing your request"

**Fix:**
1. Check backend console for error stack trace
2. Check database connection string
3. Verify MySQL is running
4. Check database `library management system` exists

### Issue 5: Empty List When Should Have Data
**Issue:** Returns `[]` but you have pending requests

**Check:**
1. Open MySQL:
```bash
mysql -u library_dev -p
USE `library management system`;
SELECT * FROM BorrowRequests WHERE Status = 'pending';
```
2. If no results: No pending requests (expected)
3. If results exist: Check status - might be 'approved' or 'rejected'

---

## 📝 Configuration Files

### Backend Configuration
**File:** `LMS Backend/Properties/launchSettings.json`
```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7076;http://localhost:5266"
    }
  }
}
```

**Database:** `LMS Backend/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DevelopmentConnection": "Server=localhost;Database=library management system;Uid=library_dev;Pwd=password123;"
  }
}
```

### Frontend Configuration
**File:** `src/app/core/services/data/generic/data.service.ts`
```typescript
protected webApiUrl: string = 'https://localhost:7076';
```

---

## 🧪 Test Endpoints

### Test 1: Swagger UI
```
URL: https://localhost:7076/swagger
Expected: See all API endpoints
```

### Test 2: Get All Requests
```
Method: GET
URL: https://localhost:7076/borrow-request/get-all
Expected: JSON array of all requests
```

### Test 3: Get Pending Requests
```
Method: GET
URL: https://localhost:7076/borrow-request/get-pending
Expected: JSON array of pending requests
```

### Test 4: Finish Request
```
Method: PUT
URL: https://localhost:7076/borrow-request/finish?id=1
Body: {}
Expected: { "success": true } or true
```

---

## 📊 Database Schema

### BorrowRequests Table
```sql
CREATE TABLE BorrowRequests (
  Id INT PRIMARY KEY AUTO_INCREMENT,
  UserId INT,
  BookISBN INT,
  Count INT DEFAULT 1,
  RequestDate DATETIME,
  Status VARCHAR(50),  -- 'pending', 'approved', 'rejected'
  FOREIGN KEY (UserId) REFERENCES Users(Id),
  FOREIGN KEY (BookISBN) REFERENCES Books(ISBN)
);
```

---

## 🆘 Still Having Issues?

Please provide:
1. Screenshot of browser console error
2. Screenshot of Network tab response
3. Backend console output (last 20 lines)
4. What OS are you using?
5. What ports show when you run `netstat -ano`?

Then we can debug from there!

---

## ✅ Success Indicators

- [ ] Backend Swagger loads at `https://localhost:7076/swagger`
- [ ] API returns JSON at `https://localhost:7076/borrow-request/get-pending`
- [ ] Frontend loads at `http://localhost:4200`
- [ ] "Pending Requests" page shows list (or empty if no requests)
- [ ] No errors in browser console
- [ ] Network tab shows 200 status for API calls
- [ ] Clicking "Finish" button updates the request
