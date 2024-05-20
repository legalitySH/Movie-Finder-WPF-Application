using System.Windows.Input;

namespace MovieFinder.Attributes
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand RegistrationCommand = new RoutedUICommand(
            "Register",
            "RegistrationCommand",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand AuthorizationCommand = new RoutedUICommand(
           "Authorization",
           "AuthorizationCommand",
    typeof(CustomCommands)
);
    }

}
