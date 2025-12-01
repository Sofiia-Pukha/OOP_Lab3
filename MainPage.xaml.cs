using System.Collections.ObjectModel;

namespace Lab3;

public partial class MainPage : ContentPage
{
    
    ObservableCollection<Book> booksCollection = new ObservableCollection<Book>();

    JsonFileService fileService = new JsonFileService();
    string currentFilePath = "";

    public MainPage()
    {
        InitializeComponent();
       
        BooksList.ItemsSource = booksCollection;
    }

   
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var newBook = new Book(); 

        var editPage = new BookEditPage(newBook, (book) =>
        {
            book.Id = booksCollection.Count + 1; 
            booksCollection.Add(book); 
            StatusLabel.Text = $"Книгу '{book.Title}' додано!";
        });

       
        await Navigation.PushModalAsync(editPage);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bookToDelete = button.CommandParameter as Book;

        if (bookToDelete != null)
        {
            bool answer = await DisplayAlert("Видалення", $"Видалити '{bookToDelete.Title}'?", "Так", "Ні");
            if (answer)
            {
                booksCollection.Remove(bookToDelete);
                StatusLabel.Text = "Книгу видалено.";
            }
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bookToEdit = button.CommandParameter as Book;

        if (bookToEdit != null)
        {
            var editPage = new BookEditPage(bookToEdit, (book) =>
            {
                
                int index = booksCollection.IndexOf(book);
                booksCollection[index] = book;

                StatusLabel.Text = $"Книгу '{book.Title}' оновлено!";
            });

            await Navigation.PushModalAsync(editPage);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
           
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, "library.json");

            await fileService.SaveToFileAsync(booksCollection.ToList(), path);

            currentFilePath = path;
            await DisplayAlert("Успіх", $"Збережено у: {path}", "ОК");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        try
        {
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, "library.json");

            if (File.Exists(path))
            {
                var loadedBooks = await fileService.ReadFromFileAsync(path);

                booksCollection.Clear();
                foreach (var book in loadedBooks)
                {
                    booksCollection.Add(book);
                }
                StatusLabel.Text = $"Завантажено {booksCollection.Count} книг.";
            }
            else
            {
                await DisplayAlert("Інфо", "Файл ще не створено. Спочатку збережіть щось.", "ОК");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        string term = SearchEntry.Text?.ToLower() ?? "";

      
        var filtered = booksCollection.Where(b =>
            b.Title.ToLower().Contains(term) ||
            b.Author.ToLower().Contains(term) ||
            b.Genre.ToLower().Contains(term)).ToList();
        

        BooksList.ItemsSource = filtered;
        StatusLabel.Text = $"Знайдено: {filtered.Count}";
    }

    private void OnResetSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
        BooksList.ItemsSource = booksCollection;
        StatusLabel.Text = "Список скинуто.";
    }

   
    private async void OnAboutClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushModalAsync(new AboutPage());
    }
}