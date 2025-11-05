using MudBlazor;

namespace CCC.Services.Notification
{
    public class NotificationService
    {
        private readonly ISnackbar snackbar;
        public NotificationService(ISnackbar _snackbar) 
        {
            snackbar = _snackbar;
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        }

        public void ShowSuccess(string message)
        {
            snackbar.Add(message, Severity.Success, opt =>
            {
                opt.ShowCloseIcon = true;
                opt.VisibleStateDuration = 3000;
                opt.HideTransitionDuration = 250;
                opt.ShowTransitionDuration = 250;
                opt.CloseAfterNavigation = true;
                opt.SnackbarVariant = Variant.Filled;
                opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            });
        }

        public void ShowInfo(string message)
        {
            snackbar.Add(message, Severity.Info, opt =>
            {
                opt.ShowCloseIcon = true;
                opt.VisibleStateDuration = 3000;
                opt.HideTransitionDuration = 250;
                opt.ShowTransitionDuration = 250;
                opt.CloseAfterNavigation = true;
                opt.SnackbarVariant = Variant.Filled;
                opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            });
        }

        public void ShowError(string message)
        {
            snackbar.Add(message, Severity.Error, opt =>
            {
                opt.ShowCloseIcon = true;
                opt.VisibleStateDuration = 9000;
                opt.HideTransitionDuration = 250;
                opt.ShowTransitionDuration = 250;
                opt.CloseAfterNavigation = true;
                opt.SnackbarVariant = Variant.Filled;
                opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            });
        }

        public void ShowWarning(string message)
        {
            snackbar.Add(message, Severity.Warning, opt =>
            {
                opt.ShowCloseIcon = true;
                opt.VisibleStateDuration = 3000;
                opt.HideTransitionDuration = 250;
                opt.ShowTransitionDuration = 250;
                opt.CloseAfterNavigation = true;
                opt.SnackbarVariant = Variant.Filled;
                opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            });
        }

    }
}
