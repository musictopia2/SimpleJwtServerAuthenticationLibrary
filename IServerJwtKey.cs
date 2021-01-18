using System.Threading.Tasks;
namespace SimpleJwtServerAuthenticationLibrary
{
    public interface IServerJwtKey
    {
        /// <summary>
        /// could use iconfiguration but is not required to.
        /// had to be non async unfortunately since i am using it for registrations.
        /// </summary>
        /// <returns></returns>
        string GetKey();
    }
}
