# Chat Application

### Overview
This is a feature-rich chat application built using .NET Core 6 for the backend and Angular for the frontend. 
The application supports real-time communication, efficient user management, and fast user search functionality.

### Features
  - **User Authentication.**
     - **Login and Registration:** Users can register and securely log in to the application.
  - **Real-Time Chat**
    - Users can send and receive messages in real-time.
    - Messages are saved asynchronously in the database to ensure performance and reliability.
  - **User Presence Indicator**
    - An online status indicator shows which users are currently active.
  - **User Search**
    - Efficiently search for users by name or email using ElasticSearch.
  - **Unread Message Indicator**
    - Displays the number of unread messages for each user.
  - **Message Seen Status**
    - Indicates whether the recipient has seen the message.

### Technologies Used
  - **Backend**
    - **.NET Core 6:** Provides a robust and scalable backend API.
    - **SignalR:** Ensures real-time communication for chat and presence updates.
    - **Apache Kafka:** Used for reliable and asynchronous message saving to the database.
  - **Frontend**
    - **Angular:** For a dynamic and responsive user interface.
  - **Search**
    - **ElasticSearch:** Powers fast and efficient user search functionality.
