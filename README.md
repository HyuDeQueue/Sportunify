# Sportunify App

Sportunify is a WPF application designed for music management. It features a login system, imports songs into a database by converting them to binary format, and uses a Database-first approach to scaffold from a MySQL schema.

## Features

- **Login System:** Secure login system for user authentication.
- **Song Import:** Import songs into the database by converting them to binary.
- **Database-First Approach:** Scaffolding from a MySQL schema to generate the necessary models and context for the application.

## Getting Started

### Prerequisites

- .NET Framework (version required for the WPF application)
- MySQL Server
- MySQL Workbench or any MySQL client for database management

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/sportunify.git
    ```

2. Open the solution in Visual Studio.

3. Restore NuGet packages:
    ```sh
    dotnet restore
    ```

4. Update the connection string in `App.config` to match your MySQL database credentials:
    ```xml
    <connectionStrings>
        <add name="SportunifyContext" connectionString="server=your_server;user=your_user;password=your_password;database=your_database" providerName="MySql.Data.MySqlClient" />
    </connectionStrings>
    ```

5. Run the application.

## Usage

1. **Login:** Start the application and log in with your credentials.
2. **Import Songs:** Use the import functionality to add songs to the database. The songs will be converted to binary and stored in the database.
3. **Manage Songs:** Access and manage your song library within the application.

## Team Members

| Name                    | Role            |
|-------------------------|-----------------|
| Đặng Quang Huy          | Developer       |
| Võ Huy Hoàng            | Developer       |
| Đặng Minh Đức           | Developer       |
| Nguyễn Anh Anh          | Developer       |
| Trần Nguyễn Việt Thành  | Developer       |

## Contributing

We welcome contributions to improve the Sportunify app. Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to our mentors and peers for their continuous support.
- Special thanks to the open-source community for their valuable resources and tools.

