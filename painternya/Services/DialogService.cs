using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using painternya.Interfaces;
using painternya.Views;

namespace painternya.Services;

public class DialogService : IDialogService
{
    private readonly Dictionary<object, Window> _openDialogs = new Dictionary<object, Window>();

    public async Task<T> ShowDialog<T>(object viewModel)
    {
        var dialog = new NewCanvasDialog();
        dialog.DataContext = viewModel;

        _openDialogs[viewModel] = dialog;
        
        var result = await dialog.ShowDialog<T>(App.Current.MainWindowInstance);
        _openDialogs.Remove(viewModel);

        return result;
    }
    
    public void SetDialogResult(object viewModel, object? result)
    {
        if (_openDialogs.TryGetValue(viewModel, out var dialog))
        {
            if (dialog is IHasDialogResult dialogWithResult)
            {
                dialogWithResult.DialogResultValue = result;
            }
        }
    }

    public void CloseDialog(object viewModel)
    {
        if (_openDialogs.TryGetValue(viewModel, out var dialog))
        {
            if (dialog is IHasDialogResult dialogWithResult && dialogWithResult.DialogResultValue != null)
            {
                dialog.Close(dialogWithResult.DialogResultValue); 
            }
            else
            {
                dialog.Close();
            }
            _openDialogs.Remove(viewModel);
        }
    }

}