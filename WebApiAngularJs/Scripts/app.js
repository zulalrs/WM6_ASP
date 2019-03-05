/// <reference path="angular.js" /> sadece intellisens in çalışmasını sağlıyor . ya bastığımızda kodların gelmesini sağlıyor.

var app = angular.module("myApp", []);
var host = "http://localhost:57167/"; // api category controllerı içindeki metodlara ulaşabilmek için oluşturacağımız url'ler için kullanacağız.

app.controller("ProductCtrl", function ($scope) {
    // 1.urunlerin içinde tutulacağı bir dizi tanımladık.
    $scope.urunler = [];
    $scope.ekleMi = false;
    $scope.sepetList = []; // Sepete eklenen ürünleri farklı bir arrayde tutmalıyız ki sepet içindeki ürünlerin varlığını kontrol edebilelim.
    $scope.sepetToplam = 0;
    // 5. localstorage dan ürünleri getirecek fonksiyon
    function init() {
        var data = JSON.parse(localStorage.getItem("urunler")); // localstorage da kayıtlı olan ürünleri getirecek ve bir data değişkenine atayacak
        $scope.urunler = data === null ? [] : data; // datanın null gelme durumunun kontrolu ve yapılacakları. null gelirse boş bir array olsun değilsede datanın kendisi gelsin.
        data = JSON.parse(localStorage.getItem("sepet"));
        $scope.sepetList = data === null ? [] : data;
        sepetHesapla();
    }
    // 3. Ekleme işlemi. html kısımda tanımlanan yeni.urunAdi ve yeni.fiyat değişkenleri scope içerisinde oluşuyor ve onları burada çağırıyorum.
    $scope.ekle = function () {
        $scope.urunler.push({ //ürünler dizisine yeni bir nesne oluşturup ekliyorum.
            id: guid(),
            urunAdi: $scope.yeni.urunAdi, // Bu nesnenin urunAdi propertysi değerini aşağıdan gelen yeni.urunAdi ndan alıyor
            fiyat: $scope.yeni.fiyat, // Bu nesnenin fiyat propertysi değerini aşağıdan gelen yeni.fiyat tan alıyor
            eklenmeZamani: new Date()

        });
        // 2.Yazdığım değerlerin ekle işleminde sonra silinmesi içinde "" değerini atıyorum. 
        $scope.yeni.urunAdi = "";
        $scope.yeni.fiyat = "";
        localStorage.setItem("urunler", JSON.stringify($scope.urunler)); // Her ekleme işleminden sonra urunleri storage a kaydedicek
    };

    // 6. Silme işlemi
    $scope.sil = function (id) {
        // ürünler arrayi içerisinde gelenin id sine eşit olan nesnenin indexini bulmak için for dongusu ile geziyoruz.
        for (var i = 0; i < $scope.urunler.length; i++) {
            var data = $scope.urunler[i];
            if (id === data.id) {
                $scope.urunler.splice(i, 1);    //Nesneyi bulduğumuzda indexi de bulmuş oluyoruz. Buldugumuz indexteki nesneyi splice ile siliyoruz. 1 ise kaç tane silmek istediğimizi belirtiyor. (i. indexteki elemandan 1 tane sil)
                break;
            }
        }
        localStorage.setItem("urunler", JSON.stringify($scope.urunler)); // localden de silm işlemi
    };

    // 7. Sepete ürün ekleme işlemleri
    $scope.sepeteekle = function (urun) {
        var varMi = false;
        // sepetList içerisinde gezerek array içerisinde gelen üründen olup olmadıgın bakıyoruz. 
        for (var i = 0; i < $scope.sepetList.length; i++) {
            var data = $scope.sepetList[i];
            if (data.id === urun.id) { // Eger varsa bool değişkeninin değerini true yapıyoruz.
                varMi = true;
                data.adet++; // Ve ürünümüzün adedini bir arttırıyoruz. adet propertysi bu fonksiyon içerisinde oluşturulmuş oldu.
                break;
            }
        }
        if (!varMi) { // Eger yoksa
            urun.adet = 1; // adet değerini 1 alıyoruz.
            $scope.sepetList.push(urun);    // Ve urunu array e ekliyoruz.
        }
        localStorage.setItem("sepet", JSON.stringify($scope.sepetList)); // Sepet için ayrı bir localStorage oluşturuyoruz.
        sepetHesapla();
    };

    // 8. Sepet hesaplama işlemleri
    function sepetHesapla() {
        $scope.sepetToplam = 0;
        for (var i = 0; i < $scope.sepetList.length; i++) {
            var data = $scope.sepetList[i];
            $scope.sepetToplam += data.adet * data.fiyat;
        }
    }

    // 9. Sepettean çıkartma işlemleri
    $scope.cikart = function (urun) {
        var index = $scope.sepetList.indexOf(urun); // viewden gonderilen ürünün indexini bulduk
        if (index > -1) // index varsa
            $scope.sepetList.splice(index, 1); // O indexteki üründen bir tane çıkart.s
        localStorage.setItem("sepet", JSON.stringify($scope.sepetList)); //localStorage ı tekrar çağır
        sepetHesapla(); // Ve sepeti tekrar hesapla
    };

    // 4. Guid üreten fonksiyon
    function guid() {
        function S4() {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        }

        // then to call it, plus stitch in '4' in the third group
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }
    init(); // Bu fonksiyonun sayfa yuklendiğinde çağırılmasını istediğimz için burada onu yazdık böylece tetiklenmeisini sağladık.
});

app.controller("CategoryCtrl", function ($scope, $http) {
    $scope.categoryList = [];
    $scope.istekVarMi = false;
    $scope.guncelleMi = false;

    // 1) Tüm kategorileri getirme işlemi
    function init() {
        $scope.istekVarMi = true;
        $http({
            method: 'GET',
            url: host + "api/category/getall"   // Method çağrımı
        }).then(function successCallback(response) {
            $scope.istekVarMi = false;
            // İki fonksiyon yazabiliyoruz ilki success durumunda ikinciside error durumunda kullanılıyor.
            // Controllerdan(return ok tan) gelen datamız ve success degeri response içindeki data da yer alıyor.Onun için data ve success e ulaşmak için response.data dedik.
            var r = response.data;
            if (r.success) { // İşlem başarılı ise datamızı categoryList arrayi içerisine aktardık.
                $scope.categoryList = r.data;
            }
            else { // Değilse de hata mesajını dondurecek.
                alert(r.message);
            }
        }, function errorCallback(response) {
            $scope.istekVarMi = false;
            console.log(response);
        });
    }

    // 2) Kategori ekleme işlemi
    $scope.ekle = function () {
        $scope.istekVarMi = true;
        $http({
            method: "POST",
            url: host + "api/category/add",
            data: $scope.yeni   // View kısmında yeni diye bir nesne oluturmuş ve propertylerini category porpertyleri ile aynı olarak vermiştik onu data olarak gonderiyoruz.
        }).then(function (rs) {
            $scope.istekVarMi = false;
            if (rs.data.success) {
                alert(rs.data.message);
                init(); // Başarılı ise initi tekrar çağıracak böylece eklenen yeni nesneyi gormuş olacağız.
            } else {
                alert("bir hata oluştu " + rs.data.message);    // Başarılı değilse hata mesajını gosterecek.
            }
            }, function (re) {
                $scope.istekVarMi = false;
            });
    };

    // 3) Kategori silme işlemi
    $scope.sil = function (id) {
        $scope.istekVarMi = true;
        $http({
            method: "DELETE",
            url: host + "api/category/delete/" + id // apicontrollerdaki delete metodunu çalıştıracak ve parametre olarak en sona yazdığımız id yi verecek.
        }).then(function (rs) {
            $scope.istekVarMi = false;
            if (rs.data.success) {
                alert(rs.data.message);
                init(); // Başarılı ise initi tekrar çağıracak böylece silinen nesne sonrası yeni listeyi gormuş olacağız.
            } else {
                alert("Bir hata oluştu " + rs.data.message);
                }
            }, function (re) {
                $scope.istekVarMi = false;
                alert(re.data.Message)  // Hata Catch teki ex.Message dan geldiği için buradaki message ın M si büyük 
            });
    };

    // 4) Guncellenecek kategoriyi bulma işlemi. Bulduktan sonra gelen kategorinin propertyleri ekrandati textboxlar içeriisnde gözükecek.
    $scope.getir = function (category) {
        $scope.guncelleMi = true;
        $scope.yeni = category; // yeni nesnesini view kısmında oluşturmuş hatta propertylerini bile orada tanımlamıştık. Parametre olarak gelen kategori nesnesini scope içerisindeki yeni nesnesine atadık. Böylece listedeki guncelleye tıklandığında veriler textboxları doldurmuş olacak.
    }

    // 5) Guncelleme işlemi
    $scope.guncelle = function () {
        $scope.istekVarMi = true;
        $http({
            method: "PUT",
            url: host + "api/category/putcategory/" + $scope.yeni.CategoryID, // url kısmında id side yazacak ve methoda parametre olarak id gidecek
            data: $scope.yeni // yeni nesnesini de data olarak gonderiyoruz methodda ikinci parametre olarak kullanılacak.
        }).then(function (rs) {
            $scope.istekVarMi = false;
            if (rs.data.success) {
                alert(rs.data.message);
                $scope.yeni = null; // textboxları temizlemek için
                $scope.guncelleMi = false; // Tekrar ekle buttonunun gözükmesi için
                init(); // Listenin son halini gormek için bu metodu burada tekrar çağırdık.
            } else {
                alert("bir hata oluştu " + rs.data.message);
                }
            }, function (re) {
                $scope.istekVarMi = false;
                console.log(re);
                alert(re.data.Message);
            });
    };
    init();
});
