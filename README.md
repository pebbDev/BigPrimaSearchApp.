# PencarianPrimaBesarMPI

## Deskripsi Singkat

PencarianPrimaMPI adalah aplikasi berbasis C# yang memanfaatkan Message Passing Interface (MPI) dan pemrograman asinkron untuk mencari bilangan prima terbesar dalam rentang yang diberikan. Aplikasi ini menggunakan pemrosesan paralel untuk mempercepat pencarian bilangan prima dengan membagi rentang pencarian ke beberapa proses.

## Nama Kelompok

1. Febriyadi (F55121082)
2. Aulia Intan Prasasti (F55121033)
3. Muh. Ikra Nur Sattya (F55121085)
4. Melsy Patricia Anggelina. E (F55121026)
5. Andi Wulan Wahyuni (F55121001)
6. Felisya Brigita Tania Wong (F55121006)

## Dokumentasi Cara Install MPI.NET di NuGet Manage

1. Buka Visual Studio.
2. Buka proyek Anda.
3. Klik kanan pada proyek di Solution Explorer, lalu pilih `Manage NuGet Packages`.
4. Pada tab `Browse`, ketik `MPI.NET`.
5. Pilih paket `MPI.NET` dari daftar hasil pencarian.
6. Klik `Install`.

## Kode yang Melakukan Proses Paralel

Berikut adalah cuplikan kode yang menunjukkan penggunaan pemrosesan paralel menggunakan MPI dan tugas asinkron untuk mencari bilangan prima terbesar:

```csharp
static async Task<long> CariPrimaTerbesarAsync(long mulai, long akhir)
{
    long primaTerbesar = 0;

    var tugas = new List<Task<long>>();
    for (long i = mulai; i <= akhir; i += 10000)
    {
        long mulaiLokal = i;
        long akhirLokal = Math.Min(i + 9999, akhir);
        tugas.Add(Task.Run(() =>
        {
            long terbesarLokal = 0;
            for (long j = mulaiLokal; j <= akhirLokal; j++)
            {
                if (ApakahPrima(j))
                {
                    terbesarLokal = j;
                }
            }
            return terbesarLokal;
        }));
    }

    var hasil = await Task.WhenAll(tugas);
    primaTerbesar = hasil.Max();

    return primaTerbesar;
}
```

## Cara Menjalankan Jumlah Proses Paralel di Terminal

1. Buka terminal atau command prompt.
2. Navigasikan ke direktori proyek Anda.
3. Jalankan aplikasi dengan perintah `mpiexec -n <jumlah_proses> <nama_aplikasi>`, sebagai contoh:

   ```sh
   mpiexec -n 4 PencarianPrimaMPI.exe
   ```

   Perintah ini akan menjalankan aplikasi dengan 4 proses paralel.

## Ucapan Terima Kasih

Terima kasih kepada semua anggota kelompok yang telah bekerja keras untuk menyelesaikan proyek ini. Kami juga mengucapkan terima kasih kepada dosen pembimbing dan semua pihak yang telah memberikan dukungan dan bantuan selama proses pengerjaan aplikasi ini.
