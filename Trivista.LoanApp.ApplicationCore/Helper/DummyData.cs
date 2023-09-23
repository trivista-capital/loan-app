using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Helper;

public class DummyData
{
    public static Customer GenerateCustomer()
    {
        return Customer.Factory.Build(Guid.NewGuid(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), 
            Faker.Phone.Number(), "Male", Faker.Identification.DateOfBirth().ToString(), Guid.NewGuid().ToString(), "Customer");
    }
}