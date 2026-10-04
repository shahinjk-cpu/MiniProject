# Waggy Pet Shop - ASP.NET Core MVC eCommerce Platform 🐾

Müasir və tamfunksiyalı ev heyvanları mağazası (Pet Shop) eCommerce veb tətbiqi. Layihə HTML/CSS şablonundan (Waggy) ASP.NET Core MVC arxitekturasına uyğunlaşdırılmış, Entity Framework Core Code-First və MS SQL LocalDB ilə verilənlər bazası təmin edilmişdir.

---

## 🚀 Texnologiya Qatı (Tech Stack)

- **Framework**: ASP.NET Core 10.0 / 8.0 (MVC)
- **Database ORM**: Entity Framework Core (Code-First, Auto-Migrations, Seed Data)
- **Database**: Microsoft SQL Server LocalDB (`(localdb)\mssqllocaldb`)
- **Təhlükəsizlik və İdentifikasiya**: ASP.NET Core Identity & Role-based Authorization (`Admin`, `Customer`)
- **API Sənədləşdirməsi**: Microsoft OpenAPI & **Scalar API Documentation** (`/scalar/v1`)
- **Frontend**: Razor Views, Bootstrap 5.3, Swiper.js, Iconify Icons, jQuery & Vanilla JS

---

## 🌟 Əsas Funksionallıqlar

### 🛍️ Müştəri Tərəfi (Storefront)
- **Əsas Səhifə (Home)**: Dinamik kateqoriyalar, xüsusi təkliflər, ən çox satılan məhsullar, bloq yazıları və rəylər karuseli.
- **Məhsul Kataloqu (Shop)**: Kateqoriya üzrə filtrləmə, axtarış, qiymət diapazonu filtri və səhifələmə (pagination).
- **Məhsul Detalı (Product Detail)**: Şəkillər, stok statusu, reytinq, oxşar məhsullar bölməsi və rəy sistemi.
- **Səbət (Cart)**: Məhsul sayının artırılması/azaldılması, kupon kodu tətbiqi, dinamik ümumi məbləğ hesablanması.
- **Sifariş (Checkout)**: Çatdırılma məlumatları, ödəniş seçimləri, sifarişin qeydə alınması və avtomatik stok azalması.
- **İstək Siyahısı (Wishlist)**: Bəyənilən məhsulları yadda saxlamaq.
- **Bloq (Blog & Details)**: Kateqoriyalı bloq məqalələri, baxış sayı və şərhlər.
- **Əlaqə (Contact Us)**: Əlaqə məlumatları və birbaşa mesaj göndərmə forması.
- **Haqqımızda (About Us) & Tez-tez Verilən Suallar (FAQs)**: Dinamik accordion və şirkət məlumatları.
- **İstifadəçi Hesabı (Account)**: Qeydiyyat, Giriş və Sifariş Tarixçəsi.

---

### 🛡️ İdarəetmə Paneli (Admin Panel - `/admin`)
- Saytın menyusunda heç bir link görünmür — yalnız birbaşa URL yazmaqla daxil olmaq mümkündür (`/admin`).
- `[Authorize(Roles = "Admin")]` ilə tam qorunur.
- **Dashboard**: Ümumi satış gəliri, sifariş sayı, məhsul sayı və oxunmamış bildirişlərin statistikası.
- **Məhsul İdarəetməsi (Products)**: Yeni məhsul əlavə etmək, redaktə, şəkil yükləmək, qiymət/stok dəyişmək və silmək.
- **Kateqoriyalar (Categories)**: Kateqoriyaların CRUD əməliyyatları.
- **Sifarişlər (Orders)**: Bütün sifarişlərin izlənməsi və statuslarının dəyişdirilməsi (*Pending*, *Processing*, *Shipped*, *Delivered*, *Cancelled*).
- **Əlaqə Mesajları (Messages)**: Saytdan göndərilən müştəri ismarıclarının oxunması və statusu.

---

### ⚡ Scalar API Sənədləşdirməsi
- İnteraktiv müasir API arayüzü: **`/scalar/v1`**
- RESTful JSON API:
  - `GET /api/products` (axtarış, kateqoriya, səhifələmə)
  - `GET /api/products/{id}`
  - `GET /api/categories`

---

## 🔑 Standart Admin Giriş Məlumatları

| Parametr | Qiymət |
| :--- | :--- |
| **Giriş Linki** | `http://localhost:5000/admin` |
| **Email** | `admin@petshop.com` |
| **Şifrə** | `Admin123!` |

---

## 💻 Quraşdırma və İşə Salma

1. Repozitoriyanı klonlayın:
   ```bash
   git clone https://github.com/shahinjk-cpu/MiniProject.git
   cd MiniProject
   ```

2. Layihə asılılıqlarını bərpa edin:
   ```bash
   dotnet restore
   ```

3. Tətbiqi işə salın (Verilənlər bazası və ilkin məlumatlar avtomatik yaranacaq):
   ```bash
   dotnet run --urls=http://localhost:5000
   ```

4. Brauzerdə açın:
   - Əsas sayt: `http://localhost:5000`
   - Admin Panel: `http://localhost:5000/admin`
   - Scalar API: `http://localhost:5000/scalar/v1`
