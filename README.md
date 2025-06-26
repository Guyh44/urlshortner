# URL Shortener

A full-stack URL shortening application that allows users to convert long URLs into short links.
Then, the users can use those short links and be redirect to the original URLs.

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
- **HTML5** - Markup and styling
- **JavaScript** - Programming language
- **CSS** - Responsive design and styling

### Backend
- **.NET 9** - Modern web framework
- **ASP.NET Core Web API** - RESTful API framework
- **SQLite** - Lightweight database to store the URLs
- **Swagger/OpenAPI** - API documentation 
- **Docker** - Run the app Localy

### Additional Tools
- **VS Code** - Code editor - recommended to run the project in
- **Node.js & npm** - Frontend package management

## Prerequisites

Before running this project, make sure you have the following installed on your system:

- **.NET 9 SDK**
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
- **Node.js** (version 14.0 or higher) via: https://nodejs.org/
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

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Guyh44/urlshortner.git
cd urlshortner
git checkout main
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
npm install react react-dom react-scripts
npm install axios
npm install bootstrap
npm install @fortawesome/fontawesome-free
npm install --save-dev @types/react @types/react-dom
```

If you're setting up from scratch, create a new React app:

```bash

npx create-react-app frontend
cd frontend
npm install axios bootstrap
```

now you are all set

## Running the Project

### Method 1: Run with docker



### Method 2: Run Frontend and Backend Separately

#### Start the Backend (.NET API)

From the project backend directory:

```bash
cd backend
dotnet run
```

The backend API will start on:
- **HTTP**: `http://localhost:5031`
- **Swagger UI**: `http://localhost:5031/swagger`

#### Start the Frontend (React)

Open a new terminal and navigate to the frontend:

```bash
cd frontend
npm start
```

The React frontend will start on `http://localhost:3000`


### Backend Endpoints

#### 1. Shorten URL
**POST** `/api/shorten`

Creates a shortened URL from a long URL.

**Request Body**:
```json
{
  "url": "https://www.example.com/very/long/url/that/needs/shortening",
  "ttl": "XXX"
}
```

**Response** (200 OK):
```json
{
  "shortUrl": "http://localhost:5000/aBc123"
}
```

#### 2. Shorten URL With Custom Code
**POST** `/api/custom/shorten`

Creates a custom shortened URL from a long URL.

**Request Body**:
```json
{
  "url": "https://www.example.com/very/long/url/that/needs/shortening",
  "customCode": "ABC123",
  "ttl": "XXX"
}
```

**Response** (200 OK):
```json
{
  "shortUrl": "http://localhost:5000/ABC123"
}
```


#### 3. Redirect to Original URL
**GET** `/{code}`

Redirects to the original URL using the short code.

**Parameters**:
- `code` (string): The 6-character short code
