using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MPI;

class PencarianPrimaMPI
{
    static bool ApakahPrima(long angka)
    {
        if (angka <= 1) return false;
        if (angka == 2) return true;
        if (angka % 2 == 0) return false;
        for (long i = 3; i <= Math.Sqrt(angka); i += 2)
        {
            if (angka % i == 0) return false;
        }
        return true;
    }

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

        try
        {
            var hasil = await Task.WhenAll(tugas);
            primaTerbesar = hasil.Max();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Terjadi pengecualian: {ex.Message}");
        }

        return primaTerbesar;
    }

    static async Task MainAsync(string[] args)
    {
        try
        {
            MPI.Environment.Run(ref args, comm =>
            {
                int peringkat = comm.Rank;
                int ukuran = comm.Size;

                long mulai = 1;
                long akhir = 1000000; // Rentang bilangan prima yang akan dicari
                long rentang = (akhir - mulai + 1) / ukuran;
                long mulaiLokal = mulai + peringkat * rentang;
                long akhirLokal = (peringkat == ukuran - 1) ? akhir : mulaiLokal + rentang - 1;

                Console.WriteLine($"Proses {peringkat} memeriksa rentang {mulaiLokal} hingga {akhirLokal}");

                var pengukurWaktu = Stopwatch.StartNew();

                // Memanggil metode asinkron dan menunggu hasilnya
                long primaTerbesar = 0;
                try
                {
                    primaTerbesar = CariPrimaTerbesarAsync(mulaiLokal, akhirLokal).GetAwaiter().GetResult();
                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        Console.WriteLine($"Proses {peringkat} mengalami pengecualian: {ex.Message}");
                    }
                }

                pengukurWaktu.Stop();
                var waktuTerlewat = pengukurWaktu.ElapsedMilliseconds;

                Console.WriteLine($"Proses {peringkat} menemukan prima terbesar {primaTerbesar} dalam {waktuTerlewat} ms");

                long primaTerbesarGlobal = comm.Reduce(primaTerbesar, Operation<long>.Max, 0);
                long waktuTerlewatMaks = comm.Reduce(waktuTerlewat, Operation<long>.Max, 0);

                // Memastikan semua proses mencapai titik ini sebelum melanjutkan
                comm.Barrier();

                if (peringkat == 0)
                {
                    Console.WriteLine($"Prima terbesar ditemukan: {primaTerbesarGlobal}");
                    Console.WriteLine($"Total waktu yang diambil oleh proses terlama: {waktuTerlewatMaks} ms");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }
}
