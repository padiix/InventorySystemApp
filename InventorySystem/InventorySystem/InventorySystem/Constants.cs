namespace InventorySystem
{
    public static class Constants
    {
        

        //TODO: Change the "10.0.2.2" part to working address, as this one is a loopback to dev machine localhost
        public const string AccountEndpoint = "http://10.0.2.2:5000/api/account";
        public const string ItemsEndpoint = "http://10.0.2.2:5000/api/items";

        //Endpoints for specific things
        public const string AccountLogin = AccountEndpoint + "/login";
        public const string AccountRegister = AccountEndpoint + "/register";

        //Success messages
        public const string DeletionSuccessful = "Pomyślnie usunięto przedmiot z bazy danych";
        public const string UpdateSuccessful = "Pomyślnie zaktualizowano przedmiot w bazie danych";
        public const string AddingItemSuccessful = "Pomyślnie dodano przedmiot do bazy danych";

        //Errors messages
        public const string FillInFieldsError = "Pewne pola nie są wypełnione, proszę, uzupełnij je!";
        public const string RegistrationError = "Podczas rejestracji wystąpił błąd";
        public const string ConnectionError = "Wystąpił błąd podczas łączenia się z API";
        public const string NoTokenError = "Brak tokena uwierzytelniającego";
        public const string ItemError = "Wystąpił błąd podczas pobierania przedmiotu z API";
        public const string ItemsError = "Wystąpił błąd podczas pobierania przedmiotów z API";
        public const string SpecificItemError = "Wystąpił błąd podczas pobierania wybranego przedmiotu z API";
        public const string EmailAndPasswordFillInError = "Uzupełnij pola \"Email\" oraz \"Hasło\" !";
        public const string UnauthorizedError = "Błędny login lub hasło!";
        public const string DeletionError = "Wystąpił błąd podczas usuwania przedmiotu z bazy danych";
        public const string UpdateItemError = "Wystąpił błąd podczas zapisywania zmian przedmiotu w bazie danych";
        public const string AddingItemError = "Wystąpił błąd podczas dodawania przedmiotu do bazy danych";
    }
}
