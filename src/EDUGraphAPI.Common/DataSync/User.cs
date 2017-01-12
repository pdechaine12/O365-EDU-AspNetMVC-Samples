namespace EDUGraphAPI.DataSync
{
    /// <summary>
    /// An instance of the class represents a user in Azure AD.
    /// </summary>
    /// <remarks>
    /// Notice that the properties used to track changes are virtual.
    /// </remarks>
    public class User
    {
        public string ObjectId { get; set; }

        public virtual string JobTitle { get; set; }

        public virtual string Department { get; set; }

        public virtual string Mobile { get; set; }
    }
}