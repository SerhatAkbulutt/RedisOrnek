using RedisOrnek;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Numerics;

internal static class Program
{
   static bool cikis = false;
    private static void Main(string[] args)
    {
        Menu();
    }

    private static void Menu()
    {
        while (!cikis)
        {
            Console.WriteLine(" ■ Yeni Kayit  =>  1");
            Console.WriteLine(" ■ Liste       =>  2");
            Console.WriteLine(" ■ Kayit Sil   =>  3");
            Console.WriteLine(" ■ Çıkış       =>  4");
            Console.WriteLine("\n ! Seçim İçin Bir Rakam Giriniz:");
            var okunan = Console.ReadLine();
            if (SayiMi(okunan))
            {
                var secim = Convert.ToInt32(okunan);
                Console.Clear();
                switch (secim)
                {
                    case 1:
                        YeniKayit();
                        break;
                    case 2:
                        Listele();
                        break;
                    case 3:
                        KayitSil();
                        break;
                    case 4:
                        cikis = true;
                        break;
                    default:
                        Console.WriteLine("! Hatali Seçim 2 sn sonra menüye yönlendirilecek...");
                        Thread.Sleep(2000);
                        break;
                }
            }
            else
            {
                Console.WriteLine("! Hatali Seçim 2 sn sonra menüye yönlendirilecek...");
                Thread.Sleep(2000);
            }
        }
    }

    private static void YeniKayit()
    {
        Console.WriteLine("\n --- Kayıt Ekranı ---  \n");
        Console.Write("TcNo:");
        string TcNo = Console.ReadLine();
        Console.Write("Ad Soyad:");
        string AdSoyad = Console.ReadLine();
        bool flag = true;
        while (flag)
        {
            Console.Write("Puan:");
            string Puan = Console.ReadLine();
            if (SayiMi(Puan))
            {
                int intPuan = Convert.ToInt32(Puan);
                if (intPuan > -1 && intPuan < 101)
                {
                    var item = new SinavSonuc
                    {
                        ID = TcNo,
                        AdSoyad = AdSoyad,
                        Puan = intPuan
                    };
                    Ekle(item);
                    flag = false;
                }
                else
                {
                    Console.WriteLine("! Hatalı giriş. Puanı [0-100] yeniden girin");
                }
            }
            else
            {
                Console.WriteLine("! Hatalı giriş. Puanı [0-100] yeniden girin");
            }

        }
    }

    private static void Ekle(SinavSonuc item)
    {
        using (RedisClient client = new RedisClient("localhost", 6379))
        {
            IRedisTypedClient<SinavSonuc> sinavSonuc = client.As<SinavSonuc>();
            sinavSonuc.Store(item);
            Console.WriteLine("İşlem Tamamlandı.");
            client.Save();
        }
        Console.WriteLine("\n\n");
        Menu();
    }
    private static void Listele()
    {
        Console.WriteLine("\n --- Sınav Sonuçları Listesi ---  \n");
        using (RedisClient client = new RedisClient("localhost", 6379))
        {
            IRedisTypedClient<SinavSonuc> sonuclar = client.As<SinavSonuc>();
            foreach (SinavSonuc sonuc in sonuclar.GetAll())
            {
                Console.WriteLine("| TcNo: {0,-12} | Ad Soyad: {1,-20} | Sınav Sonucu: {2,-2} |", sonuc.ID, sonuc.AdSoyad, sonuc.Puan);
            }
        }
        Console.WriteLine("\n\n");
        Menu();
    }
    private static void KayitSil()
    {
        string SecilenTcNo = "";
        Console.Write("Kaydı Silinecek Kişinin TcNo sunu Giriniz:");
        SecilenTcNo = Console.ReadLine();
        using (RedisClient client = new RedisClient("localhost", 6379))
        {
            IRedisTypedClient<SinavSonuc> sonuclar = client.As<SinavSonuc>();
            if (sonuclar.GetAll().Where(x => x.ID == SecilenTcNo).ToList().Count > 0)
            {
                sonuclar.DeleteById(SecilenTcNo);
                client.Save();
                Console.WriteLine("Kayıt Silindi.\n\n");
            }

            else
            {
                Console.WriteLine("\n! ! ! Böyle bir kayda rastlanmadı.\n\n");
            }
        }
    }
    public static bool SayiMi(this string text)
    {
        foreach (char chr in text)
        {
            if (!Char.IsNumber(chr)) return false;
        }
        return true;
    }
}