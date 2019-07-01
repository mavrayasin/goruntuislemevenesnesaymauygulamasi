using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace AForge.NET_Blob_Counter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
        }

        Blob[] blobs;
        int sayac;
        int toplamSekilSayisi;
        Bitmap kaynakResim;
        Bitmap siyahBeyazResim;

        //ilk önce bu butona tıklanmalı
        private void button1_Click(object sender, EventArgs e)
        {
            sayac = 1;
            kaynakResim = (Bitmap)System.Drawing.Image.FromFile("sample2.jpg");

            //Otsu Threshold uygulandı
            OtsuThreshold otsuFiltre = new OtsuThreshold();
            //resim eğer renkliyse önce griye çeviriyor sonra filtre uyguluyor
            //resim zaten griyse direk filtreyi uyguluyor
            siyahBeyazResim = otsuFiltre.Apply(kaynakResim.PixelFormat != PixelFormat.Format8bppIndexed ? Grayscale.CommonAlgorithms.BT709.Apply(kaynakResim) : kaynakResim);

            BlobCounterBase bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinHeight = 5;
            bc.MinWidth = 5;

            bc.ProcessImage(siyahBeyazResim);
            blobs = bc.GetObjectsInformation();
            label2.Text = "Toplam Şekil Sayısı = " + bc.ObjectsCount.ToString();
            toplamSekilSayisi = bc.ObjectsCount;

            /*aşağıdaki for döngüsü yerine buradaki koşul ifadesi kullanısaydı eğer, 
            resimdeki en büyük şekil bulunacaktı.
            if (blobs.Length > 0)
            {
                bc.ExtractBlobsImage(siyahBeyazResim, blobs[0], true);
            }
            Bitmap xxx = blobs[0].Image.ToManagedImage();
            */

            //bütün şekiller bulunuyor
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                if (blobs.Length > 0)
                {

                    bc.ExtractBlobsImage(siyahBeyazResim, blobs[i], true);
                    pictureBox1.Image = siyahBeyazResim;
                    pictureBox1.Refresh();
                }
            }
        }

        //bulunan şekiller arasında sırasıyla dolaşılabilir
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = blobs[sayac - 1].Image.ToManagedImage();
            label1.Text = "Şekil " + sayac.ToString() + " / " + toplamSekilSayisi.ToString();

            if (sayac == toplamSekilSayisi)
            {
                sayac = 0;
            }
            sayac++;
        }

        //kenarlardaki siyah bölgeleri temizler
        private void button3_Click(object sender, EventArgs e)
        {
            //filtre oluşturuldu
            ExtractBiggestBlob filtre = new ExtractBiggestBlob();
            //filtre uygulandı
            Bitmap temizResim = filtre.Apply((Bitmap)pictureBox1.Image);
            pictureBox1.Image = temizResim;
        }

        //her şekle farklı bir renk verir
        private void button4_Click(object sender, EventArgs e)
        {
            //filtre oluşturuldu
            ConnectedComponentsLabeling filtre = new ConnectedComponentsLabeling();
            Bitmap boyaliResim = filtre.Apply((Bitmap)pictureBox1.Image);
            pictureBox1.Image = boyaliResim;
        }

        //verilen sınır değerlerden küçük şekilleri göz ardı eder
        private void button5_Click(object sender, EventArgs e)
        {
            //filtre oluşturuldu
            BlobsFiltering filtre = new BlobsFiltering();
            //filtre ayarları yapıldı
            filtre.CoupledSizeFiltering = true;
            filtre.MinWidth = 70;
            filtre.MinHeight = 70;

            //burada bazıları ApplyInPlace() fonksiyonunu kullanıyor
            //ben Apply() fonksiyonunu kullanıyorum
            //ApplyInPlace() fonksiyonunu daha önce hiç kullanmadım, nasıl çalıştığınıda çözemedim zaten:)
            Bitmap newImage = filtre.Apply((Bitmap)pictureBox1.Image); 
            pictureBox1.Image = newImage;
        }
    }
}
