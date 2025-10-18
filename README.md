# **Information Retrieval System**

This project is a web application built with C\# and ASP.NET Core MVC that demonstrates fundamental concepts of Information Retrieval. It allows users to input a collection of text documents and generates two key data structures: a **Term-Document Incidence Matrix** and an **Inverted Index**. The application also supports executing simple boolean queries (AND, OR, NOT) on the indexed documents.

## **Features**

* **Dual Document Input:** Submit documents by either pasting raw text or uploading multiple .txt files.  
* **Term-Document Matrix Generation:** Creates and displays a boolean matrix indicating the presence (1) or absence (0) of terms in each document.  
* **Inverted Index Generation:** Builds and displays an inverted index, mapping each term to a postings list of documents it appears in.  
* **Boolean Query Support:** Perform simple AND, OR, and NOT queries on the indexed collection to find matching documents.  
* **Clean & Responsive UI:** A straightforward, single-page interface styled with Bootstrap.

## **Technology Stack**

* **Language:** C# 8.0  
* **Framework:** ASP.NET Core MVC (built on .NET Core 3.1)  
* **Frontend:** HTML, Bootstrap, and vanilla JavaScript.  
* **Architecture:** 3-Tier (Presentation, Business Logic, Data Access).

## **Architecture**

The application follows a clean 3-tier architecture to ensure a strong separation of concerns, making it maintainable and scalable.

* InformationRetrieval.PL **(Presentation Layer):** An ASP.NET Core MVC project that handles all user interaction, routing, and view rendering. It is responsible for displaying the user interface.  
* InformationRetrieval.BLL **(Business Logic Layer):** A .NET Class Library containing the core information retrieval algorithms. It handles tokenization, index building, and query processing, with no dependencies on web technologies.  
* InformationRetrieval.DAL **(Data Access Layer):** A placeholder .NET Class Library. In this project, all data is processed in-memory. This layer is included to demonstrate architectural best practices and allow for future database integration.

## **Getting Started**

Follow these instructions to get a copy of the project up and running on your local machine.

### **Prerequisites**

You will need the following software installed on your machine:

1. **.NET Core 3.1 SDK** (or .NET 8.0).  
2. One of the following development environments:  
   * **Visual Studio 2019** (or newer) with the "ASP.NET and web development" workload installed.  
   * **Visual Studio Code** with the official C\# extension installed.

### **Running the Application**

#### **Option 1: Using Visual Studio (Recommended)**

1. Clone the repository:  
   git clone https://github.com/Mohamed-Hi4am/InformationRetrieval.git

2. Open the InformationRetrieval.sln solution file in Visual Studio.  
3. Press F5 or the green "Run" button. Visual Studio will automatically restore dependencies, build the solution, and launch the application in your default web browser.

#### **Option 2: Using the .NET CLI**

1. Clone the repository:  
   git clone https://github.com/Mohamed-Hi4am/InformationRetrieval.git

2. Open a terminal or command prompt and navigate to the root folder of the repository.  
3. Restore the project dependencies:  
   dotnet restore

4. Run the application, specifying the Presentation Layer project:  
   dotnet run \--project src/InformationRetrieval.PL/InformationRetrieval.PL.csproj

5. Open a web browser and navigate to the URL displayed in the terminal (e.g., https://localhost:5001).

## **How to Use**

1. **Choose an Input Method:** Select either the "Paste Text" or "Upload Files" tab.  
2. **Provide Documents:**  
   * For pasting, specify the number of documents, and text areas will appear.  
   * For uploading, select one or more .txt files.  
3. **Show Results:** Click the "Show Results" button.  
4. **View Results:** The Term-Document Matrix and Inverted Index will be displayed on the page.  
5. **Perform a Query:** Use the query form at the bottom of the page to search the documents. The results will appear below the form.

## **Tokenization Rules**

The text processing engine follows a specific set of rules for tokenization:

* **Case Insensitive:** All text is converted to lowercase.  
* **Abbreviations:** Terms with periods (e.g., u.s.a.) are treated as a single token.  
* **Hyphenated Words:** Words connected by hyphens (e.g., state-of-the-art) are split into separate tokens (state, of, the, art).  
* **No Stop Words:** All terms are indexed; no stop words are removed.
