# Timeline of People

A Blazor WebAssembly application that displays a chronological visualization of notable individuals.

## Features

- ✅ **Chronological Timeline**: People are displayed in order by birth date
- ✅ **Add/Edit/Delete**: Full CRUD operations for managing timeline entries
- ✅ **Search & Filter**: Search by name or description
- ✅ **Local Storage**: Data persists in browser localStorage
- ✅ **Responsive Design**: Works on desktop and mobile devices
- ✅ **Sample Data**: Preloaded with famous physicists dataset

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/Netonia/Timeline.git
   cd Timeline/Timeline
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to the URL shown in the console (typically `http://localhost:5033`)

### Building for Production

To build the application for deployment:

```bash
dotnet publish -c Release
```

The output will be in `bin/Release/net9.0/publish/wwwroot/` and can be deployed to any static hosting service like GitHub Pages.

## Usage

1. **View Timeline**: The timeline displays people chronologically by birth date
2. **Add Person**: Fill in the form on the left (desktop) or top (mobile) and click "Add Person"
3. **Edit Person**: Click the "Edit" button on any timeline entry to modify it
4. **Delete Person**: Click the "Delete" button on any timeline entry to remove it
5. **Search**: Use the search bar to filter people by name or description

## Technology Stack

- **Frontend**: Blazor WebAssembly (.NET 9)
- **Styling**: Bootstrap 5 + Custom CSS
- **Storage**: Browser localStorage API
- **Hosting**: Static file hosting (GitHub Pages compatible)

## Project Structure

```
Timeline/
├── Components/          # Reusable Blazor components
│   ├── PersonForm.razor       # Add/Edit form component
│   └── TimelineDisplay.razor  # Timeline visualization component
├── Models/             # Data models
│   └── Person.cs             # Person entity
├── Services/           # Business logic
│   └── TimelineService.cs    # Data management and localStorage
├── Pages/              # Application pages
│   └── Home.razor            # Main application page
├── wwwroot/            # Static assets
│   └── css/
│       └── app.css           # Custom styles
└── Program.cs          # Application entry point
```

## License

This project is open source and available under the MIT License.