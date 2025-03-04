using System.Collections.ObjectModel;

namespace Computer_Hardware_Strore
{
    public class ProductCard
    {
        public int ProduktId { get; set; }
        public string Nazvanie { get; set; }
        public string Opisanie { get; set; }
        public decimal Tsena { get; set; }
        public int IdKategorii { get; set; }
        public int KolichestvoTovarov { get; set; }
        public string UrlIzobrazheniya { get; set; }
        public string Kategoria { get; set; }
        public ObservableCollection<string> Categories { get; set; }
    }
}