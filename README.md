
# GDP Office Hours

 The project application provides details like schedule, booking an appointment by the student and sending message if the advisor is out of station. Dropping an email or booking appointment and sending messages directly, and easy access of the appointments for the advisor wherever he is. Providing security for the data throughout the process.
This app was created using ASP.NET by Section 2 of CSIS 44692, Graduate Directed Project, at Northwest Missouri State University during the Fall Semester of 2018. It was created under the direction of Dr. Aziz Fellah , afellah@nwmissouri.edu.

#1. User’s guide for the users and client:

. The user needs to select the faculty that he wants to interact. Then the user can view the office hours of the faculty. 
. Once the user clicks on the signin, the system redirects the user to Microsoft organisation page where he needs to provide his . credentials.
. If the user is student, the system redirects to student home page where he can view the office hours of the faculty.
. Then the user can schedule the appointment, send/view  messages, capture the notes/submit the photo of the paper that he wants.
. If the user is a faculty, then he is provided with options to choose i.e., student view or faculty view.
. If the user clicks on the student view he can get the view of the student and can perform operations specified in the above steps.
. If the user chooses faculty view then he can view the appointments made by students,Messages sent by the students and the papers submitted by them.
. The user can change the availability status in his view.
. The user needs to logout once he is done using the application.

#2. Installation guide:
. Download and install Microsoft visual studio. Refer the below link:
https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2017
. Download and install Microsoft SQL server 2017 professional with ssms/management studio . Refer the below link:
https://www.microsoft.com/en-us/sql-server/sql-server-download
. Register the sample on the app registration portal.
. Create a new app at apps.dev.microsoft.com, or follow these detailed steps. Make sure to Copy down the Application ID assigned to your app, you'll need it soon.
. Add the Web platform for your app.
. Enter the correct Redirect URI. The redirect uri indicates to Azure AD where authentication responses should be directed - the default is https://localhost:4279/.
. Add a new application secret via the "Generate new password", and save the result in a temporary location - you'll need it in the next step.

#3. Execution guide:

. To get the working files into a local computer, clone the repository.
       Repository link: https://github.com/RevanthVallamsetty/GDPOfficeHours
       Steps to Clone:
. Click the repository link mentioned above, select Clone this repository.
. Copy the clone command (either the SSH format or the HTTPS)
. From a terminal window, change to the local directory where you want to clone your repository.
. Enter the command git clone and the following link https://github.com/RevanthVallamsetty/GDPOfficeHours
. Enter the github credentials and the repository will be cloned into the local system.
. Click the officehours.sln file in visual studio and right click on the project and click on build.
. Open the Package manager console present in tools and run below command in console:
              nuget restore OfficeHours.sln.
      
. Create a Database in SQL server and change the connection string in the Web.config file in the project. Then open the SQL server object explorer and connect to the database you created on the local machine before
      
. In the web.config file , Find the app key ida:ClientSecret and replace the value with the application secret you saved while installation/registering app in portal..Find the app key ida:ClientId and replace the value with the Application ID from the app registration portal, again in installation/registering app in portal..If you changed the base URL of the sample, find the app key ida:RedirectUri and replace the value with the new base URL of the project.
        
 Then run the application by clicking on the button on the top menu bar labelled IIS express.This runs the app in the local IIS server. App will listen on http://localhost:4279 




