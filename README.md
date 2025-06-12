# URL Shortener

A full-stack URL shortening application that allows users to convert long URLs into short links.

## Features

- **URL Shortening**: Convert long URLs into short, manageable links
- **URL Redirection**: Automatically redirect users from short URLs to original URLs
- **Duplicate Prevention**: Returns existing short URL if the same long URL is submitted again
- **Modern React UI**: Clean and responsive user interface
- **Swagger Documentation**: Interactive API documentation
- **SQLite Database**: Lightweight, file-based database for storing URL mappings
- **Input Validation**: URL format validation on both frontend and backend

## Technologies Used

### Frontend
- **React.js** - User interface library
- **HTML5/CSS3** - Markup and styling
- **JavaScript** - Programming language
- **CSS** - Responsive design and styling

### Backend
- **.NET 9** - Modern web framework
- **ASP.NET Core Web API** - RESTful API framework
- **SQLite** - Lightweight database
- **Entity Framework Core** - Database ORM
- **Swagger/OpenAPI** - API documentation
- **System.Data.SQLite** - SQLite data provider

### Additional Tools
- **VS Code** - Code editor
- **Node.js & npm** - Frontend package management

## 📋 Prerequisites

Before running this project, make sure you have the following installed on your system:

- **.NET 9 SDK** or later
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
- **Node.js** (version 14.0 or higher)
- **npm** (Node Package Manager)
- **Visual Studio Code**
- **Git** (for cloning the repository)

### Verify Installation
Open a command prompt/terminal and run:
```bash
dotnet --version
node --version
npm --version
```
You should see .NET 9.0.0 or later, and Node.js 14.0 or later.

## 🔧 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Guyh44/urlshortner.git
cd urlshortner
git checkout main
```

## 🏗️ Project Structure

```
urlshortner/
├── Controllers/
│   └── UrlShortenerController.cs    # API endpoints
├── Services/
│   ├── AddDB.cs                     # Database operations
│   └── ShortURL.cs                  # URL shortening logic
├── Models/
│   └── RandomString.cs              # Random string generation
├── Properties/
├── bin/                             # Compiled binaries
├── obj/                             # Build artifacts
├── ClientApp/                       # React frontend
│   ├── public/
│   │   └── index.html
│   ├── src/
│   │   ├── components/
│   │   ├── services/
│   │   ├── App.js
│   │   ├── App.css
│   │   └── index.js
│   ├── package.json
│   └── package-lock.json
├── appsettings.json                 # Configuration
├── Program.cs                       # Application entry point
├── urlshortner.csproj              # Project file
├── urlshortner.sln                 # Solution file
├── urlshortner.http                # HTTP test requests
└── UrlDataBase.db                  # SQLite database file
```

### 2. Backend Setup (.NET API)

Navigate to the project root directory and restore the .NET packages:

```bash
dotnet restore
```

#### Backend Dependencies

The following NuGet packages are required and will be installed:

```bash
# Core packages (install if creating from scratch)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.5
dotnet add package System.Data.SQLite --version 1.0.119
dotnet add package Swashbuckle.AspNetCore --version 8.1.4
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.5
dotnet add package Microsoft.AspNetCore.SpaServices.Extensions --version 9.0.5
```

#### Build the Backend

```bash
dotnet build
```

### 3. Frontend Setup (React)

Navigate to the React frontend directory:

```bash
cd frontend
```

#### Install Frontend Dependencies

```bash
npm install
```

#### Frontend Dependencies

The following npm packages will be installed:

```bash
# Core React dependencies
npm install react react-dom react-scripts

# HTTP client for API requests
npm install axios

# Additional UI/styling (if used)
npm install bootstrap
npm install @fortawesome/fontawesome-free

# Development dependencies
npm install --save-dev @types/react @types/react-dom
```

If you're setting up from scratch, create a new React app:

```bash
# Only if creating React app from scratch
npx create-react-app frontend
cd frontend
npm install axios bootstrap
```

## 🚀 Running the Project

### Method 1: Run Frontend and Backend Separately

#### Start the Backend (.NET API)

From the project root directory:

```bash
dotnet run
```

The backend API will start on:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger`

#### Start the Frontend (React)

Open a new terminal and navigate to the React app:

```bash
cd frontend
npm start
```

The React frontend will start on `http://localhost:3000`

### Method 2: Run Both with Single Command (if configured)

If your project is configured to run both frontend and backend together:

```bash
dotnet run
```

This will automatically start both the .NET API and serve the React build files.

### Method 3: Production Build

To create a production build of the React app:

```bash
cd frontend
npm run build
```

Then run the .NET application:

```bash
dotnet run --configuration Release
```

## 📚 API Documentation

### Backend Endpoints

#### Base URL: `http://localhost:5000/api` or `https://localhost:5001/api`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/shorten` | Create a shortened URL |
| POST | `/api/custom/shorten` | Create a custom shortened URL |
| GET | `/{shortCode}` | Redirect to original URL |

#### 1. Shorten URL
**POST** `/api/shorten`

Creates a shortened URL from a long URL.

**Request Body**:
```json
{
  "url": "https://www.example.com/very/long/url/that/needs/shortening"
}
```

**Response** (200 OK):
```json
{
  "shortUrl": "http://localhost:5000/aBc123"
}
```

**Response** (400 Bad Request):
```json
"Invalid URL."
```


#### 2. Redirect to Original URL
**GET** `/{code}`

Redirects to the original URL using the short code.

**Parameters**:
- `code` (string): The 6-character short code

**Response**:
- **302 Found**: Redirects to the original URL
- **404 Not Found**: "Short URL not found"

### Usage Examples

#### Using the Web Interface

1. **Access the Application**: Open your browser and navigate to `http://localhost:3000`
2. **Shorten a URL**: 
   - Enter a long URL in the input field
   - Click "Shorten URL"
   - Copy the generated short URL
3. **Use the Short URL**: 
   - Share the short URL anywhere
   - When clicked, it will redirect to the original URL

#### Using cURL (API Testing)

**Shorten a URL**:
```bash
curl -X POST "http://localhost:5000/api/shorten" \
     -H "Content-Type: application/json" \
     -d '{"url": "https://www.google.com"}'
```

**Access shortened URL**:
```bash
curl -L "http://localhost:5000/aBc123"
```

#### Using PowerShell

**Shorten a URL**:
```powershell
$body = @{
    url = "https://www.example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/shorten" `
                  -Method POST `
                  -Body $body `
                  -ContentType "application/json"
```

#### Using JavaScript/Fetch API (Frontend Integration)

**Shorten a URL**:
```javascript
fetch('http://localhost:5000/api/shorten', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify({
        url: 'https://www.example.com'
    })
})
.then(response => response.json())
.then(data => console.log(data.shortUrl));
```

## 🗄️ Database Schema

The application uses SQLite with the following table structure:

### URLS Table
| Column      | Type     | Description                    |
|-------------|----------|--------------------------------|
| Id          | INTEGER  | Primary key (auto-increment)  |
| OriginalUrl | TEXT     | The original long URL         |
| ShortCode   | TEXT     | The 6-character short code    |
| CreatedAt   | DATETIME | Timestamp of creation         |

**Constraints**:
- `OriginalUrl`: UNIQUE, NOT NULL
- `ShortCode`: UNIQUE, NOT NULL

The database file is automatically created at: `UrlDataBase.db`

## 🐛 Troubleshooting

### Common Issues

1. **Backend Port Already in Use**
   - Change the port in `Properties/launchSettings.json`
   - Or specify a different port: `dotnet run --urls "http://localhost:5001"`

2. **Frontend Port Already in Use**
   - The React app will automatically suggest an alternative port
   - Or set a specific port: `PORT=3001 npm start`

3. **Database Permission Issues**
   - Ensure the application has write permissions to the database directory
   - Check that the database path in `AddDB.cs` is accessible

4. **Invalid URL Error**
   - Ensure URLs include the protocol (http:// or https://)
   - Example: Use `https://google.com` instead of `google.com`

5. **.NET Version Issues**
   - Verify .NET 9 SDK is installed: `dotnet --version`
   - Update project target framework if needed

6. **Node.js/npm Issues**
   - Verify Node.js is installed: `node --version`
   - Clear npm cache: `npm cache clean --force`
   - Delete `node_modules` and reinstall: `rm -rf node_modules && npm install`

7. **CORS Errors**
   - Ensure the .NET API is configured to allow requests from React app
   - Check that the frontend is making requests to the correct backend URL

8. **React Build Issues**
   - Clear React cache: `npm start -- --reset-cache`
   - Delete `node_modules` and `package-lock.json`, then reinstall

## ⚙️ Configuration

### Backend Configuration

The .NET application configuration is managed through `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Frontend Configuration

Create a `.env` file in the `ClientApp` directory for environment variables:

```env
REACT_APP_API_URL=http://localhost:5000
REACT_APP_BASE_URL=http://localhost:3000
```

### Database Configuration

By default, the SQLite database file is created as `UrlDataBase.db` in the project root.

To change the database location, update the path in `Services/AddDB.cs`:

```csharp
private static string connectionString = "Data Source=UrlDataBase.db";
```

### URL Configuration

The short URL base is configured in `Services/ShortURL.cs`:
```csharp
private static string baseUrl = "http://harpaz.url/";
```

You can modify this to match your domain when deploying to production.

## 🔧 Development

### Running in Development Mode

**Backend**:
```bash
dotnet run --environment Development
```

**Frontend**:
```bash
cd ClientApp
npm start
```

### Building for Production

**Backend**:
```bash
dotnet publish -c Release -o ./publish
```

**Frontend**:
```bash
cd ClientApp
npm run build
```

### Running Tests

**Backend Tests**:
```bash
dotnet test
```

**Frontend Tests**:
```bash
cd ClientApp
npm test
```

### Debugging

Enable detailed logging by updating `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

## 🔒 Security Considerations

- **Input Validation**: URLs are validated on both frontend and backend
- **SQL Injection Protection**: Parameterized queries prevent SQL injection
- **CORS Configuration**: Properly configured for cross-origin requests
- **HTTPS Support**: Backend supports HTTPS in production
- **No Authentication**: This is a basic implementation without user authentication
- **Rate Limiting**: Consider implementing rate limiting for production use

## 📈 Future Enhancements

- **User Authentication**: Add user accounts and URL management dashboard
- **Analytics Dashboard**: Track click counts, geographic data, and usage statistics
- **Custom Short Codes**: Allow users to specify custom short codes
- **Expiration Dates**: Add URL expiration functionality
- **QR Code Generation**: Generate QR codes for short URLs
- **Bulk Operations**: Support bulk URL shortening
- **API Rate Limiting**: Implement comprehensive rate limiting
- **URL Categories**: Organize URLs by categories or tags
- **Social Media Integration**: Direct sharing to social platforms