using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class Customer: BaseEntity<Guid>
{
    internal Customer() { }

    private Customer(Guid id, string firstName, string lastName, string email, string phoneNumber, string sex, string dob, string roleId, string userType)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Sex = sex;
        Dob = dob;
        Created = DateTime.UtcNow;
        RoleId = roleId;
        Created = DateTime.UtcNow;
        UserType = userType;
    }
    
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Sex { get; set; }

    public string Dob { get; set; }

    public string Occupation { get; set; }
    
    public string Address { get; set; }
    
    public string Country { get; set; }
    
    public string State { get; set; }
    
    public string City { get; set; }
    
    public string Bvn { get; set; }
    
    public string PostCode { get; set; }

    public string RoleId { get; set; }
    
    public string UserType { get; set; }
    
    public int MbsRequestStatementResponseCode { get; private set; }
    
    public string MbsBankStatement { get; private set; }
    
    public string MbsBankStatementTicketAndPassword { get; private set; }
    
    public string BankStatementAnalysis { get; private set; }

    public LoaneeTypes LoaneeTypes { get; set; }
    public CustomerRemitterInformation CustomerRemitterInformation { get; set; }
    public ProfilePicture? ProfilePicture { get; private set; }
    public ICollection<LoanRequest> LoanRequests { get; set; } = new List<LoanRequest>();

    public string? Location { get; private set; }

    public List<Ticket> Tickets { get; set; }
        = new List<Ticket>();

    public Customer SetCustomerRemittance(CustomerRemitterInformation customerRemitterInformation)
    {
        CustomerRemitterInformation = customerRemitterInformation;
        return this;
    }
    
    public Customer IsRemittaUser(bool isRemita)
    {
        this.CustomerRemitterInformation.IsRemittaUser = isRemita;
        return this;
    }
    
    public Customer SetMbsCode(int code)
    {
        this.MbsRequestStatementResponseCode = code;
        return this;
    }
    
    public Customer SetMbsBankStatement(string statement)
    {
        this.MbsBankStatement = statement;
        return this;
    }
    
    public Customer SetBankStatementAnalysis(string analysis)
    {
        this.BankStatementAnalysis = analysis;
        return this;
    }

    // public Customer SetRemittaMandate(GenerateRemittaMandateResponseDto mandate)
    // {
    //     this.CustomerRemitterInformation.Mandate = mandate;
    //     return this;
    // }
    
    public Customer SetFirstName(string firstName)
    {
        FirstName = firstName;
        return this;
    }
    
    public Customer SetLastName(string lastName)
    {
        LastName = lastName;
        return this;
    }
    
    public Customer SetMiddleName(string middleName)
    {
        MiddleName = middleName;
        return this;
    }
    
    public Customer SetEmail(string email)
    {
        Email = email;
        return this;
    }
    
    public Customer SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        return this;
    }
    
    public Customer SetSex(string sex)
    {
        Sex = sex;
        return this;
    }
    
    public Customer SetDob(string dob)
    {
        Dob = dob;
        return this;
    }
    
    public Customer SetOccupation(string occupation)
    {
        Occupation = occupation;
        return this;
    }
    
    public Customer SetAddress(string address)
    {
        Address = address;
        return this;
    }
    
    public Customer SetCountry(string country)
    {
        Country = country;
        return this;
    }
    
    public Customer SetState(string state)
    {
        State = state;
        return this;
    }
    
    public Customer SetCity(string city)
    {
        City = city;
        return this;
    }
    
    public Customer SetBvn(string bvn)
    {
        Bvn = bvn;
        return this;
    }
    
    public Customer SetPostCode(string postcode)
    {
        PostCode = postcode;
        return this;
    }
    
    public Customer SetProfilePicture(ProfilePicture profilePicture)
    {
        this.ProfilePicture = profilePicture;
        return this;
    }

    public Customer SetLocation(string location)
    {
        this.Location = location;
        return this;
    }

    public Customer SetMbsBankStatementTicketAndPassword(string mbsBankStatementTicketAndPassword)
    {
        this.MbsBankStatementTicketAndPassword = mbsBankStatementTicketAndPassword;
        return this;
    }

    public class Factory
    {
        public static Customer Build(Guid id, string firstName, string lastName, string email, string phoneNumber, string sex,
            string dob, string roleId, string userType)
        {
            return new Customer(id, firstName, lastName, email, phoneNumber, sex, dob, roleId, userType);
        }
    }
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}