namespace BookRental.Constants;

public static class Messages
{
    #region Auth
    public const string YouDontHavePermission = "You don't have permission";
    public const string MissingUserId = "Missing userId";
    public const string InvalidCredentials = "Invalid credentials";
    public const string RoleMissing = "Role missing";
    public const string UserNotFound = "User not found";
    public const string InvalidPassword = "Invalid password";
    #endregion

    #region Tenant
    public const string MissingTenantId = "Missing tenantId";
    public const string InvalidTenantId = "Invalid tenantId";
    #endregion

    #region Books
    public const string BookDeletedSuccessfully = "Book deleted successfully";
    public const string BookUpdatedSuccessfully = "Book updated successfully";
    public const string BookRentedSuccessfully = "Book rented successfully";
    public const string BookReturnedSuccessfully = "Book returned successfully";
    public const string BookNotFound = "Book not found";
    #endregion

    #region Validation
    public const string ColumnMustBeSpecified = "Column must be specified";
    #endregion
}