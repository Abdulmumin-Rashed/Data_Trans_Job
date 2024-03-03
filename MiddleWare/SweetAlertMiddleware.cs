using Data_Trans_Job.IService.IRepositories;

namespace Data_Trans_Job.MiddleWare
{
    public class SweetAlertMiddleware
    {
        private readonly RequestDelegate _next;

        public SweetAlertMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork  unitOfWork)
        {
            await _next(context);

            if (context.Request.Method == "POST" && context.Request.Path == "/Admin/Admin/Create")
            {
                // Get the user ID from the request
                var userId = context.Request.Form["Id"];

                // Delay the notification by 5 minutes
                await Task.Delay(TimeSpan.FromSeconds(1));

                // Retrieve the user from the repository
                var user = await unitOfWork.Admin.GetUserById(userId);

                // Trigger SweetAlert notification
                if (user != null)
                {
                    var notificationMessage = $"New user '{user.FirstName} {user.LastName}' created";
                    // Your code to trigger the SweetAlert notification, e.g., using SignalR or other client-side mechanism
                }
                else
                {
                    // Handle the case where the user is not found or null
                    var notificationMessage = "New user created"; // Or any default message
                }
                // Your code to trigger the SweetAlert notification, e.g., using SignalR or other client-side mechanism
            }
        }
    }
}
