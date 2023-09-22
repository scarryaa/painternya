using System.Threading.Tasks;

namespace painternya.Interfaces;

public interface IDialogService
{
    Task<T> ShowDialog<T>(object viewModel);
    void SetDialogResult(object viewModel, object result);
    void CloseDialog(object viewModel);
}