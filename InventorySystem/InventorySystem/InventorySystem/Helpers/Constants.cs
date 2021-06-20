namespace InventorySystem
{
    public static class Constants
    {
        //Success messages
        public const string DeletionSuccessful = "Pomyślnie usunięto przedmiot z bazy danych";
        public const string UpdateSuccessful = "Pomyślnie zaktualizowano przedmiot w bazie danych";
        public const string AddingItemSuccessful = "Pomyślnie dodano przedmiot do bazy danych";

        //User errors messages
        public const string FillInFieldsError = "Pewne pola nie są wypełnione, proszę, uzupełnij je!";
        public const string NullReturnedError = "API nie zwróciło danych.";
        public const string NotExpectedError = "Podczas wykonywania zgłoszenia do API wystąpił nieprzewidziany wyjątek";
        public const string RegistrationError = "Podczas procesu rejestracji wystąpił błąd";
        public const string ConnectionError = "Wystąpił błąd podczas łączenia się z API";
        public const string ApiRejectionError = "Zgłoszenie zostało odrzucone prez API";
        public const string ExpiredTokenError = "Token autoryzacyjny użytkownika stracił ważność";
        public const string NoTokenError = "Brak tokena uwierzytelniającego";
        public const string ItemError = "Wystąpił błąd podczas pobierania przedmiotu z API";
        public const string ItemsError = "Wystąpił błąd podczas pobierania przedmiotów z API";
        public const string SpecificItemError = "Wystąpił błąd podczas pobierania wybranego przedmiotu z API";
        public const string EmailAndPasswordFillInError = "Uzupełnij pola \"Email\" oraz \"Hasło\" !";
        public const string UnauthorizedError = "Błędny login lub hasło!";
        public const string DeletionError = "Wystąpił błąd podczas usuwania przedmiotu z bazy danych";
        public const string UpdateItemError = "Wystąpił błąd podczas zapisywania zmian przedmiotu w bazie danych";
        public const string AddingItemError = "Wystąpił błąd podczas dodawania przedmiotu do bazy danych";

        //Console errors messages
        public const string Console_NoTokenError = "No token found";
        public const string Console_NullReturnedError = "No data was returned from API";
    }
}