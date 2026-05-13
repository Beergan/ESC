using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public interface ISweetAlertService
{
    Task Loading(string message, string  header = null);

    Task Alert(string message, string header = null);

    Task Error(string message, string header = null);

    Task Close();

    Task<bool> ConfirmDelete(string message, string header = null);    
}