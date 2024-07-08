using FibonacciHeap;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        GMapOverlay markeri = new GMapOverlay("markeri");
        StreamReader citanje;
        List<Vrh> listamarkera = new List<Vrh>();
        Dictionary<int, Vrh> sviVrhovi;
        bool imapocetnimarker = false;
        bool podatciUcitani;
        Vrh pocetak;
        Vrh kraj;
        int maxBrojMarkera = 2;
        int brojmarkera = 0;
        bool AlgoritamSpreman;
        GMapOverlay RutaOverlay = new GMapOverlay("rutaOverlay");
        public class Vrh
        {
            public int RoadID { get; set; }
            public double pocetakX { get; set; }
            public double pocetakY { get; set; }
            public double krajX { get; set; }
            public double krajY { get; set; }
            public double duljinauMetrima { get; set; }
            public int brzina { get; set; }
            public int ogranicenjeBrzine { get; set; }
            public int tip { get; set; }
            public int smjer { get; set; }
            public double zracnaUdaljenost { get; set; }
            public string nazivUlice { get; set; }
            public List<int> susjedniLink { get; set; }
            public List<int> linkoviPrijeZaBiDijkstru { get; set; }
            public string status { get; set; }
            public List<double> exact_points { get; set; }
            public GMapMarker marker { get; set; }

            public double tezina { get; set; }
            public Vrh prethodni { get; set; }
            public bool obraden { get; set; }
        }
        public Form1()
        {
            sviVrhovi = new Dictionary<int, Vrh>();
            InitializeComponent();
            this.Resize += new EventHandler(MapaResize);
            try
            {
                citanje = new StreamReader("CompleteMireoMapV2.txt");
                citanje.ReadLine();
                while (!citanje.EndOfStream)
                {
                    string s = citanje.ReadLine().Replace('.', ',');
                    string[] l = s.Split(';');
                    int RoadID = Convert.ToInt32(l[0]);
                    double pocetakX = Convert.ToDouble(l[1]);
                    double pocetakY = Convert.ToDouble(l[2]);
                    double krajX = Convert.ToDouble(l[3]);
                    double krajY = Convert.ToDouble(l[4]);
                    double duljinauMetrima = Convert.ToDouble(l[5]);
                    int brzina = Convert.ToInt32(l[6]);
                    int ogranicenjeBrzine = Convert.ToInt32(l[7]);
                    int tip = Convert.ToInt32(l[8]);
                    int smjer = Convert.ToInt32(l[9]);
                    double zracnaUdaljenost = Convert.ToDouble(l[10]);
                    string nazivUlice = Convert.ToString(l[11]);
                    string status = Convert.ToString(l[13]);
                    Vrh jedanVrh = new Vrh
                    {
                        RoadID = RoadID,
                        pocetakX = pocetakX,
                        pocetakY = pocetakY,
                        krajX = krajX,
                        krajY = krajY,
                        duljinauMetrima = duljinauMetrima,
                        brzina = brzina,
                        ogranicenjeBrzine = ogranicenjeBrzine,
                        tip = tip,
                        smjer = smjer,
                        zracnaUdaljenost = zracnaUdaljenost,
                        nazivUlice = nazivUlice,
                        status = status
                    };
                    jedanVrh.susjedniLink = new List<int>();
                    if (!string.IsNullOrEmpty(l[12]))
                    {
                        string[] susjedni = l[12].Trim().Split('|');
                        for (int i = 0; i < susjedni.Length; i++)
                        {
                            jedanVrh.susjedniLink.Add(Convert.ToInt32(susjedni[i]));

                        }
                    }
                    jedanVrh.linkoviPrijeZaBiDijkstru = new List<int>();
                    if (!string.IsNullOrEmpty(l[15]))
                    {
                        string[] prethodni = l[15].Trim().Split('|');
                        for (int i = 0; i < prethodni.Length; i++)
                        {
                            jedanVrh.linkoviPrijeZaBiDijkstru.Add(Convert.ToInt32(prethodni[i]));
                        }
                    }
                    sviVrhovi[jedanVrh.RoadID] = jedanVrh;
                    //Vrhovi.exact_points = new List<double>();
                    //string [] ep = l[14].Split('|');
                    //for (int i = 0; i< ep.Length; i++)
                    //{
                    //    Vrhovi.exact_points.Add(Convert.ToDouble(ep[i]));
                    //}

                }
                citanje.Close();
                // MessageBox.Show("Uspješno učitani podaci!");
                podatciUcitani = true;
                //foreach (Vrh vc in sviVrhovi.Values)
                //{
                //    foreach (int linkID in vc.susjedniLink)
                //    {
                //        Vrh vn = sviVrhovi[linkID];
                //        vn.linkoviPrijeZaBiDijkstru.Add(vc.RoadID);
                //    }
                //}

                //citanje = new StreamReader("CompleteMireoMap.txt");
                //StreamWriter pisanje = new StreamWriter("CompleteMireoMapV2.txt");
                //string line=citanje.ReadLine();
                //pisanje.WriteLine(line);
                //while (!citanje.EndOfStream)
                //{
                //    line = citanje.ReadLine();
                //    string s = line.Replace('.', ',');
                //    string[] l = s.Split(';');
                //    int RoadID = Convert.ToInt32(l[0]);
                //    line += ";";
                //    Vrh v = sviVrhovi[RoadID];
                //    for(int ii=0;ii<v.linkoviPrijeZaBiDijkstru.Count;ii++) {
                //        int linkID = v.linkoviPrijeZaBiDijkstru[ii];
                //        if(ii==0)
                //        {
                //            line += linkID;
                //        }
                //        else
                //        {
                //            line += "|"+linkID;
                //        }
                //    }
                //    pisanje.WriteLine(line);
                //}
                //citanje.Close();
                //pisanje.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške! " + ex.Message);
            }

        }
        private void MapaResize(object sender, EventArgs e)
        {
            int margin = 10;
            gMapControl1.Width = this.ClientSize.Width - 2 * margin;
            gMapControl1.Height = this.ClientSize.Height - 2 * margin;
            gMapControl1.Location = new Point(margin, margin);

        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            double lat = 45.815011; // gepgrafska sirina Zagreb
            double lng = 15.981919; // geografksa duzina Zagreb 
            gMapControl1.Position = new GMap.NET.PointLatLng(lat, lng);
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 70;
            gMapControl1.Zoom = 12;
            gMapControl1.ShowCenter = false;
            gMapControl1.CanDragMap = true;
            gMapControl1.DragButton = MouseButtons.Left;

        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            markeri.Markers.Clear();
            brojmarkera = 0;
            listamarkera.Clear();
            imapocetnimarker = false;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private Vrh PronadiNajbliziLink(PointLatLng point)
        {
            Vrh v3 = null;
            double minUdaljenost = double.MaxValue;
            foreach (Vrh v4 in sviVrhovi.Values)
            {
                double udaljenost = getDistanceFromPointToClosestPointOnLine(v4.pocetakX, v4.pocetakY, v4.krajX, v4.krajY, point.Lng, point.Lat);
                if (udaljenost < minUdaljenost)
                {
                    minUdaljenost = udaljenost;
                    v3 = v4;
                }
            }
            return v3;
        }
        public static double getDistanceFromPointToClosestPointOnLine(double lx1, double ly1, double lx2, double ly2, double px, double py)
        {
            //Vektorski racuna najblizu posctku
            double[] vec_l1P = new double[2] { px - lx1, py - ly1 };
            double[] vec_l1l2 = new double[2] { lx2 - lx1, ly2 - ly1 };

            double mag = Math.Pow(vec_l1l2[0], 2) + Math.Pow(vec_l1l2[1], 2);
            double prod = vec_l1P[0] * vec_l1l2[0] + vec_l1P[1] * vec_l1l2[1];
            double normDist = prod / mag;

            double clX = lx1 + vec_l1l2[0] * normDist;
            double clY = ly1 + vec_l1l2[1] * normDist;

            //Ukoliko je projkekcije izvan granica linije, korigirati na najblizu tocku na liniji
            double minLX = Math.Min(lx1, lx2);
            double minLY = Math.Min(ly1, ly2);
            double maxLX = Math.Max(lx1, lx2);
            double maxLY = Math.Max(ly1, ly2);
            if (clX < minLX)
            {
                clX = minLX;
            }
            if (clY < minLY)
            {
                clY = minLY;
            }

            if (clX > maxLX)
            {
                clX = maxLX;
            }
            if (clY > maxLY)
            {
                clY = maxLY;
            }
            //Vrati zracnu udaljenost u metrima izmedu najblize tocke na liniji i zadane tocke
            return airalDistHaversine(px, py, clX, clY);
        }

        //Pomocna metoda koja racuna zračunu udaljenost u metrima između dvije lokacije koristeći model zemlje kao kugla
        public static double airalDistHaversine(double lon1, double lat1, double lon2, double lat2)
        {

            double R = 6371000; // metres
            double phi1 = lat1 * Math.PI / 180; // φ, λ in radians
            double phi2 = lat2 * Math.PI / 180;
            double deltaphi = (lat2 - lat1) * Math.PI / 180;
            double deltaLambda = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaphi / 2) * Math.Sin(deltaphi / 2) +
                      Math.Cos(phi1) * Math.Cos(phi2) *
                      Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;
            return d;

        }
        private void pocetniMarker(PointLatLng tocka, Vrh v)
        {
            var marker = new GMarkerGoogle(tocka, GMarkerGoogleType.blue_pushpin);
            v.marker = marker;
            gMapControl1.Overlays.Add(markeri);
            markeri.Markers.Add(marker);
            MessageBox.Show("ID ovog linka je: " + pocetak.RoadID);
        }
        private void zavrsniMarker(PointLatLng tocka, Vrh v)
        {
            var marker = new GMarkerGoogle(tocka, GMarkerGoogleType.red_pushpin);

            v.marker = marker;
            gMapControl1.Overlays.Add(markeri);
            markeri.Markers.Add(marker);
            MessageBox.Show("ID ovog linka je: " + kraj.RoadID);
        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 2)
                {
                    if (brojmarkera < maxBrojMarkera)
                    {
                        PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                        if (imapocetnimarker == false && podatciUcitani)
                        {
                            pocetak = PronadiNajbliziLink(point);
                            pocetniMarker(point, pocetak);
                            listamarkera.Add(pocetak);
                            imapocetnimarker = true;
                            brojmarkera++;
                        }
                        else if (podatciUcitani)
                        {
                            kraj = PronadiNajbliziLink(point);
                            zavrsniMarker(point, kraj);
                            listamarkera.Add(kraj);
                            AlgoritamSpreman = true;
                            brojmarkera++;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nemoguće je postaviti više markera!");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                brojmarkera = 0;
            }

        }
        private void btnAstar_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaAStar = AstarAlgoritam(pocetak, kraj);
                    if (rutaAStar != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaAStar.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private RutaSPP AstarAlgoritam(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            Dictionary<Vrh, Vrh> dosliIZ = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> Gvrijednost = new Dictionary<Vrh, double>();
            Dictionary<Vrh, double> fVrijednost = new Dictionary<Vrh, double>();
            FibonacciHeap<Vrh, double> heap = new FibonacciHeap<Vrh, double>(double.MaxValue);
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> traziVrh = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();

            Gvrijednost[pocetak] = 0;
            fVrijednost[pocetak] = heuristickaVrijednost(pocetak, kraj);
            FibonacciHeapNode<Vrh, double> pocetni = new FibonacciHeapNode<Vrh, double>(pocetak, fVrijednost[pocetak]);
            heap.Insert(pocetni);
            traziVrh[pocetak] = pocetni;

            while (!heap.IsEmpty())
            {
                FibonacciHeapNode<Vrh, double> trenutniNode = heap.RemoveMin();
                Vrh trenutni = trenutniNode.Data;
                traziVrh.Remove(trenutni);

                if (trenutni.Equals(kraj))
                {
                    DateTime vEnd = DateTime.Now;
                    RutaSPP rutaAStar = new RutaSPP();
                    rutaAStar.listaLinkova = new List<int>();
                    rutaAStar.udaljenostStvarna = 0;
                    rutaAStar.udaljenostIzračunata = Gvrijednost[trenutni];
                    rutaAStar.opis = "A star";
                    rutaAStar.udaljenostHeuristika = Math.Round(heuristickaVrijednost(pocetak, kraj), 2);
                    rutaAStar.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaAStar.opis);
                    Vrh trenutniVrh = trenutni;
                    rutaAStar.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;

                    double xp = 0, yp = 0, xk = 0, yk = 0;
                    while (dosliIZ.ContainsKey(trenutniVrh))
                    {
                        rutaAStar.listaLinkova.Add(trenutniVrh.RoadID);
                        trenutniVrh = dosliIZ[trenutniVrh];
                    }
                    rutaAStar.listaLinkova.Add(pocetak.RoadID);

                    for (int i = 0; i < rutaAStar.listaLinkova.Count; i++)
                    {
                        Vrh tr = sviVrhovi[rutaAStar.listaLinkova[i]];
                        if (tr != kraj)
                        {
                            rutaAStar.udaljenostStvarna += tr.duljinauMetrima;
                        }
                        if (i == 0)
                        {
                            Vrh trenutni1 = sviVrhovi[rutaAStar.listaLinkova[i + 1]];
                            if (equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.pocetakX, trenutni1.pocetakY) ||
                                equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.krajX, trenutni1.krajY))
                            {
                                xp = tr.krajX;
                                yp = tr.krajY;
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            else
                            {
                                xp = tr.pocetakX;
                                yp = tr.pocetakY;
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            rutaAStar.rutaNaKarti.Points.Add(new PointLatLng(yp, xp));
                            rutaAStar.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        else
                        {
                            if (equalPoints(tr.pocetakX, tr.pocetakY, xp, yp))
                            {
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            else
                            {
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            rutaAStar.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        xp = xk;
                        yp = yk;
                    }
                    rutaAStar.rutaNaKarti.Stroke = new Pen(Color.HotPink, 3);
                    NacrtajRutu(rutaAStar);
                    return rutaAStar;
                }
                foreach (int id in trenutni.susjedniLink)
                {
                    Vrh susjedni = sviVrhovi[id];
                    double privremenaGvrijednost = Gvrijednost[trenutni] + trenutni.duljinauMetrima;

                    if (!Gvrijednost.ContainsKey(susjedni) || privremenaGvrijednost < Gvrijednost[susjedni])
                    {
                        Gvrijednost[susjedni] = privremenaGvrijednost;
                        fVrijednost[susjedni] = Gvrijednost[susjedni] + heuristickaVrijednost(susjedni, kraj);
                        dosliIZ[susjedni] = trenutni;

                        if (!traziVrh.ContainsKey(susjedni))
                        {
                            FibonacciHeapNode<Vrh, double> susjedniNode = new FibonacciHeapNode<Vrh, double>(susjedni, fVrijednost[susjedni]);
                            heap.Insert(susjedniNode);
                            traziVrh[susjedni] = susjedniNode;
                        }
                        else
                        {
                            FibonacciHeapNode<Vrh, double> postojeciNode = traziVrh[susjedni];
                            heap.DecreaseKey(postojeciNode, fVrijednost[susjedni]);
                        }
                    }
                }
            }

            return null;
        }


        private double heuristickaVrijednost(Vrh trenutni, Vrh cilj)
        {
            return airalDistHaversine((trenutni.pocetakX + trenutni.krajX) / 2, (trenutni.pocetakY + trenutni.krajY) / 2, (cilj.pocetakX + cilj.krajX) / 2, (cilj.pocetakY + cilj.krajY) / 2);
        }
        private void NacrtajRutu(RutaSPP rutaCrtanje)
        {

            gMapControl1.Overlays.Add(RutaOverlay);
            RutaOverlay.Routes.Add(rutaCrtanje.rutaNaKarti);
            //gMapControl1.ZoomAndCenterRoutes(rutaCrtanje.opis);
        }


        public RutaSPP Dijkstra(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            Dictionary<int, FibonacciHeapNode<Vrh, double>> sviUHeapu = new Dictionary<int, FibonacciHeapNode<Vrh, double>>();
            FibonacciHeap<Vrh, double> heap = new FibonacciHeap<Vrh, double>(double.MaxValue);
            FibonacciHeapNode<Vrh, double> pocetakHeapNode = new FibonacciHeapNode<Vrh, double>(pocetak, 0);

            foreach (Vrh vrhovi in sviVrhovi.Values)
            {
                vrhovi.tezina = double.MaxValue;
                vrhovi.obraden = false;
                vrhovi.prethodni = null;
                FibonacciHeapNode<Vrh, double> node = new FibonacciHeapNode<Vrh, double>(vrhovi, vrhovi.tezina);
                sviUHeapu[vrhovi.RoadID] = node;
                if (vrhovi != pocetak)
                {
                    heap.Insert(node);
                }
            }

            pocetak.tezina = 0;
            heap.Insert(pocetakHeapNode);
            sviUHeapu[pocetak.RoadID] = pocetakHeapNode;

            while (!heap.IsEmpty())
            {
                FibonacciHeapNode<Vrh, double> trenutniNode = heap.RemoveMin();
                Vrh trenutni = trenutniNode.Data;
                trenutni.obraden = true;

                if (trenutni.Equals(kraj))
                {
                    DateTime vEnd = DateTime.Now;
                    RutaSPP rutaDijkstra = new RutaSPP();
                    rutaDijkstra.listaLinkova = new List<int>();
                    rutaDijkstra.udaljenostStvarna = 0;
                    rutaDijkstra.udaljenostIzračunata = kraj.tezina;
                    rutaDijkstra.opis = "Dijkstra";
                    rutaDijkstra.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;
                    rutaDijkstra.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaDijkstra.opis);

                    Vrh trenutniVrh = kraj;

                    while (trenutniVrh != null)
                    {
                        rutaDijkstra.listaLinkova.Insert(0, trenutniVrh.RoadID);
                        if (trenutniVrh.prethodni != null)
                        {
                            rutaDijkstra.udaljenostStvarna += trenutniVrh.duljinauMetrima;
                            double xp, yp, xk, yk;
                            if (equalPoints(trenutniVrh.pocetakX, trenutniVrh.pocetakY, trenutniVrh.prethodni.pocetakX, trenutniVrh.prethodni.pocetakY) ||
                               equalPoints(trenutniVrh.pocetakX, trenutniVrh.pocetakY, trenutniVrh.prethodni.krajX, trenutniVrh.prethodni.krajY))
                            {
                                xp = trenutniVrh.krajX;
                                yp = trenutniVrh.krajY;
                                xk = trenutniVrh.pocetakX;
                                yk = trenutniVrh.pocetakY;
                            }
                            else
                            {
                                xp = trenutniVrh.pocetakX;
                                yp = trenutniVrh.pocetakY;
                                xk = trenutniVrh.krajX;
                                yk = trenutniVrh.krajY;
                            }
                            rutaDijkstra.rutaNaKarti.Points.Insert(0, new PointLatLng(yp, xp));
                            rutaDijkstra.rutaNaKarti.Points.Insert(0, new PointLatLng(yk, xk));

                        }
                        else
                        {
                            rutaDijkstra.rutaNaKarti.Points.Insert(0, new PointLatLng(trenutniVrh.krajY, trenutniVrh.krajX));
                            rutaDijkstra.rutaNaKarti.Points.Insert(0, new PointLatLng(trenutniVrh.pocetakY, trenutniVrh.pocetakX));
                        }

                        trenutniVrh = trenutniVrh.prethodni;
                    }

                    rutaDijkstra.rutaNaKarti.Stroke = new Pen(Color.Red, 3);
                    NacrtajRutu(rutaDijkstra);
                    return rutaDijkstra;
                }

                foreach (int id in trenutni.susjedniLink)
                {
                    if (sviVrhovi.ContainsKey(id))
                    {
                        Vrh susjedni = sviVrhovi[id];
                        if (susjedni.obraden)
                        {
                            continue;
                        }

                        double privremenaUdaljenost = trenutni.tezina + trenutni.duljinauMetrima;

                        if (privremenaUdaljenost < susjedni.tezina)
                        {
                            susjedni.tezina = privremenaUdaljenost;
                            susjedni.prethodni = trenutni;

                            FibonacciHeapNode<Vrh, double> susjedniNode = sviUHeapu[susjedni.RoadID];
                            heap.DecreaseKey(susjedniNode, susjedni.tezina);
                        }
                    }
                }
            }

            return null;
        }

        private void btnDijkstra_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaDijkstra = Dijkstra(pocetak, kraj);
                    if (rutaDijkstra != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n" + rutaDijkstra.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private RutaSPP DijkstraKnez(Vrh pocetak, Vrh kraj)
        //{
        //    DateTime vStart = DateTime.Now;
        //    Dictionary<int, FibonacciHeapNode<Vrh, double>> sviUHeapu = new Dictionary<int, FibonacciHeapNode<Vrh, double>>();
        //    FibonacciHeap<Vrh, double> heap = new FibonacciHeap<Vrh, double>(double.MaxValue);
        //    FibonacciHeapNode<Vrh, double> pocetakHeapNode = null;
        //    foreach (Vrh vReset in sviVrhovi.Values)
        //    {
        //        vReset.tezina = double.MaxValue;
        //        vReset.obraden = false;
        //        vReset.prethodni = null;
        //        FibonacciHeapNode<Vrh, double> node = new FibonacciHeapNode<Vrh, double>(vReset, vReset.tezina);
        //        if (vReset == pocetak)
        //        {
        //            vReset.tezina = 0;
        //            pocetakHeapNode = node;
        //        }
        //        else
        //        {
        //            heap.Insert(node);
        //        }
        //        sviUHeapu[vReset.RoadID] = node;
        //    }
        //    int i = 0;
        //    Vrh v1;
        //    while (!heap.IsEmpty())
        //    {
        //        FibonacciHeapNode<Vrh, double> najbolji = null;
        //        if (i == 0)
        //        {
        //            najbolji = pocetakHeapNode;
        //            i++;
        //        }
        //        else
        //        {
        //            najbolji = heap.RemoveMin();
        //        }
        //        najbolji.Data.obraden = true;
        //        if (najbolji.Data.RoadID == kraj.RoadID)
        //        {

        //            break;
        //        }
        //        if (!heap.IsEmpty())
        //        {
        //            v1 = najbolji.Data;
        //            foreach (int linkID in v1.susjedniLink)
        //            {
        //                if (sviVrhovi.ContainsKey(linkID))
        //                {
        //                    Vrh v2 = sviVrhovi[linkID];
        //                    if (v2.obraden)
        //                    {
        //                        continue;
        //                    }
        //                    if (v2.tezina > v1.tezina + v1.duljinauMetrima)
        //                    {
        //                        FibonacciHeapNode<Vrh, double> nodeToUpdate = sviUHeapu[v2.RoadID];
        //                        v2.tezina = v1.tezina + v1.duljinauMetrima;
        //                        v2.prethodni = v1;
        //                        heap.DecreaseKey(nodeToUpdate, v2.tezina);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    DateTime vEnd = DateTime.Now;
        //    //Izrada puta            
        //    RutaSPP rutaKnez = new RutaSPP();






        //    rutaKnez.listaLinkova = new List<int>();
        //    rutaKnez.udaljenostStvarna = 0;
        //    rutaKnez.udaljenostIzračunata = kraj.tezina;
        //    rutaKnez.opis = "DijkstraKnez";
        //    rutaKnez.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;
        //    rutaKnez.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaKnez.opis);
        //    Vrh trenutni = kraj;
        //    do
        //    {
        //        if (trenutni != kraj)
        //        {
        //            rutaKnez.udaljenostStvarna += trenutni.duljinauMetrima;
        //        }
        //        rutaKnez.listaLinkova.Insert(0, trenutni.RoadID);
        //        rutaKnez.rutaNaKarti.Points.Add(new PointLatLng(trenutni.pocetakY, trenutni.pocetakX));
        //        rutaKnez.rutaNaKarti.Points.Add(new PointLatLng(trenutni.krajY, trenutni.krajX));
        //        trenutni = trenutni.prethodni;
        //    } while (trenutni != null);

        //    //rutaKnez.udaljenostStvarna += pocetak.duljinauMetrima;
        //    //rutaKnez.rutaNaKarti.Points.Add(new PointLatLng(trenutni.pocetakY, trenutni.pocetakX));
        //    //rutaNaKarti.Points.Add(new PointLatLng(trenutni.krajY, trenutni.krajX));
        //    rutaKnez.rutaNaKarti.Stroke = new Pen(Color.Green, 3);
        //    NacrtajRutu(rutaKnez);
        //    return rutaKnez;
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (AlgoritamSpreman && brojmarkera == 2)
        //        {

        //            RutaSPP rutaDijkstra = DijkstraKnez(pocetak, kraj);
        //            if (rutaDijkstra != null)
        //            {
        //                MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaDijkstra.Detalji());

        //            }
        //            else
        //            {
        //                MessageBox.Show("Ruta nije pronađena");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void btnObrisiRutu_Click(object sender, EventArgs e)
        {
            RutaOverlay.Routes.Clear();
        }

        private RutaSPP GreedyBestFirstSearch(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            Dictionary<Vrh, Vrh> dosliIZ = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> traziVrh = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();
            FibonacciHeap<Vrh, double> heap = new FibonacciHeap<Vrh, double>(double.MaxValue);

            double heuristika = heuristickaVrijednost(pocetak, kraj);
            FibonacciHeapNode<Vrh, double> pocetniNode = new FibonacciHeapNode<Vrh, double>(pocetak, heuristika);
            heap.Insert(pocetniNode);
            traziVrh[pocetak] = pocetniNode;

            while (!heap.IsEmpty())
            {
                FibonacciHeapNode<Vrh, double> trenutniNode = heap.RemoveMin();
                Vrh trenutni = trenutniNode.Data;

                if (trenutni.Equals(kraj))
                {
                    DateTime vEnd = DateTime.Now;
                    RutaSPP rutaGBFS = new RutaSPP();
                    rutaGBFS.listaLinkova = new List<int>();
                    rutaGBFS.udaljenostStvarna = 0;
                    rutaGBFS.udaljenostIzračunata = 0;
                    rutaGBFS.udaljenostHeuristika = Math.Round(heuristika, 3);
                    rutaGBFS.opis = "GBFS";
                    rutaGBFS.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;
                    rutaGBFS.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaGBFS.opis);

                    Vrh trenutniVrh = kraj;

                    while (trenutniVrh != null)
                    {
                        rutaGBFS.listaLinkova.Insert(0, trenutniVrh.RoadID);
                        if (dosliIZ.ContainsKey(trenutniVrh))
                        {
                            Vrh prethodniVrh = dosliIZ[trenutniVrh];
                            rutaGBFS.udaljenostStvarna += trenutniVrh.duljinauMetrima;
                            double xp, yp, xk, yk;
                            if (equalPoints(trenutniVrh.pocetakX, trenutniVrh.pocetakY, prethodniVrh.pocetakX, prethodniVrh.pocetakY) ||
                                equalPoints(trenutniVrh.pocetakX, trenutniVrh.pocetakY, prethodniVrh.krajX, prethodniVrh.krajY))
                            {
                                xp = trenutniVrh.krajX;
                                yp = trenutniVrh.krajY;
                                xk = trenutniVrh.pocetakX;
                                yk = trenutniVrh.pocetakY;
                            }
                            else
                            {
                                xp = trenutniVrh.pocetakX;
                                yp = trenutniVrh.pocetakY;
                                xk = trenutniVrh.krajX;
                                yk = trenutniVrh.krajY;
                            }
                            rutaGBFS.rutaNaKarti.Points.Insert(0, new PointLatLng(yp, xp));
                            rutaGBFS.rutaNaKarti.Points.Insert(0, new PointLatLng(yk, xk));
                        }
                        else
                        {
                            rutaGBFS.rutaNaKarti.Points.Insert(0, new PointLatLng(trenutniVrh.krajY, trenutniVrh.krajX));
                            rutaGBFS.rutaNaKarti.Points.Insert(0, new PointLatLng(trenutniVrh.pocetakY, trenutniVrh.pocetakX));
                        }

                        trenutniVrh = dosliIZ.ContainsKey(trenutniVrh) ? dosliIZ[trenutniVrh] : null;
                    }

                    rutaGBFS.rutaNaKarti.Stroke = new Pen(Color.Blue, 3);
                    NacrtajRutu(rutaGBFS);
                    return rutaGBFS;
                }

                foreach (int id in trenutni.susjedniLink)
                {
                    Vrh susjedni = sviVrhovi[id];

                    if (!traziVrh.ContainsKey(susjedni))
                    {
                        double heuristikaSusjeda = heuristickaVrijednost(susjedni, kraj);
                        FibonacciHeapNode<Vrh, double> susjedniNode = new FibonacciHeapNode<Vrh, double>(susjedni, heuristikaSusjeda);
                        traziVrh[susjedni] = susjedniNode;
                        dosliIZ[susjedni] = trenutni;
                        heap.Insert(susjedniNode);
                        heap.DecreaseKey(susjedniNode, heuristikaSusjeda);
                    }
                }
            }

            return null;
        }

        private void btnGBFS_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaGBFS = GreedyBestFirstSearch(pocetak, kraj);
                    if (rutaGBFS != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaGBFS.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private RutaSPP BidirectionalDijkstra(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            Dictionary<Vrh, Vrh> dosliIZPocetak = new Dictionary<Vrh, Vrh>();

            Dictionary<Vrh, Vrh> obradeniPocetak = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> mapaVrijednostiOdPocetka = new Dictionary<Vrh, double>();
            FibonacciHeap<Vrh, double> heapOdPocetka = new FibonacciHeap<Vrh, double>(double.MaxValue);
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> mapHeapVrhPocetak = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();

            Dictionary<Vrh, Vrh> obradenKraj = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> mapaVrijednostiOdKraja = new Dictionary<Vrh, double>();
            FibonacciHeap<Vrh, double> heapOdKraja = new FibonacciHeap<Vrh, double>(double.MaxValue);
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> mapHeapVrhKraj = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();



            Dictionary<Vrh, Vrh> dosliIZKraj = new Dictionary<Vrh, Vrh>();

            FibonacciHeapNode<Vrh, double> pocetniHeap = new FibonacciHeapNode<Vrh, double>(pocetak, 0);
            heapOdPocetka.Insert(pocetniHeap);
            mapHeapVrhPocetak[pocetak] = pocetniHeap;
            mapaVrijednostiOdPocetka[pocetak] = 0;

            FibonacciHeapNode<Vrh, double> krajHeap = new FibonacciHeapNode<Vrh, double>(kraj, -kraj.duljinauMetrima);
            mapHeapVrhKraj[kraj] = krajHeap;
            heapOdKraja.Insert(krajHeap);
            mapaVrijednostiOdKraja[kraj] = -kraj.duljinauMetrima;

            Vrh zajednickiVrh = null;
            double najboljaVrijednost = double.MaxValue;
            while (!heapOdKraja.IsEmpty() && !heapOdPocetka.IsEmpty())
            {
                if (!heapOdPocetka.IsEmpty())
                {
                    FibonacciHeapNode<Vrh, double> trenutniNodeOdPocetka = heapOdPocetka.RemoveMin();
                    obradeniPocetak[trenutniNodeOdPocetka.Data] = trenutniNodeOdPocetka.Data;
                    Vrh trenutniOdPocetka = trenutniNodeOdPocetka.Data;
                    if (mapaVrijednostiOdKraja.ContainsKey(trenutniOdPocetka))
                    {
                        double ukupnaVrijednost = mapaVrijednostiOdPocetka[trenutniOdPocetka] + mapaVrijednostiOdKraja[trenutniOdPocetka] + trenutniOdPocetka.duljinauMetrima;
                        if (ukupnaVrijednost < najboljaVrijednost)
                        {
                            najboljaVrijednost = ukupnaVrijednost;
                            zajednickiVrh = trenutniOdPocetka;
                        }
                        if (obradeniPocetak.ContainsKey(trenutniOdPocetka) && obradenKraj.ContainsKey(trenutniOdPocetka))
                        {
                            break;
                        }
                    }
                    foreach (int id in trenutniOdPocetka.susjedniLink)
                    {
                        Vrh susjedni = sviVrhovi[id];
                        if (obradeniPocetak.ContainsKey(susjedni))
                        {
                            continue;
                        }
                        double privremenaUdaljenost = mapaVrijednostiOdPocetka[trenutniOdPocetka] + trenutniOdPocetka.duljinauMetrima;
                        if (!mapaVrijednostiOdPocetka.ContainsKey(susjedni) || privremenaUdaljenost < mapaVrijednostiOdPocetka[susjedni])
                        {
                            mapaVrijednostiOdPocetka[susjedni] = privremenaUdaljenost;
                            dosliIZPocetak[susjedni] = trenutniOdPocetka;

                            if (!mapHeapVrhPocetak.ContainsKey(susjedni))
                            {
                                FibonacciHeapNode<Vrh, double> susjedniNodePocetak = new FibonacciHeapNode<Vrh, double>(susjedni, privremenaUdaljenost);
                                heapOdPocetka.Insert(susjedniNodePocetak);
                                mapHeapVrhPocetak[susjedni] = susjedniNodePocetak;
                            }
                            else
                            {
                                FibonacciHeapNode<Vrh, double> postojeciNodePocetak = mapHeapVrhPocetak[susjedni];
                                heapOdPocetka.DecreaseKey(postojeciNodePocetak, privremenaUdaljenost);
                            }
                        }
                    }

                }
                if (!heapOdKraja.IsEmpty())
                {
                    FibonacciHeapNode<Vrh, double> trenutniNodeOdKraja = heapOdKraja.RemoveMin();
                    Vrh trenutniOdKraja = trenutniNodeOdKraja.Data;
                    obradenKraj[trenutniOdKraja] = trenutniOdKraja;
                    if (mapaVrijednostiOdPocetka.ContainsKey(trenutniOdKraja))
                    {
                        double ukupnaVrijednost = mapaVrijednostiOdPocetka[trenutniOdKraja] + mapaVrijednostiOdKraja[trenutniOdKraja] + trenutniOdKraja.duljinauMetrima;
                        if (ukupnaVrijednost < najboljaVrijednost)
                        {
                            najboljaVrijednost = ukupnaVrijednost;
                            zajednickiVrh = trenutniOdKraja;
                        }
                        if (obradeniPocetak.ContainsKey(trenutniOdKraja) && obradenKraj.ContainsKey(trenutniOdKraja))
                        {
                            break;
                        }
                    }
                    foreach (int id in trenutniOdKraja.linkoviPrijeZaBiDijkstru)
                    {
                        Vrh susjedni = sviVrhovi[id];
                        if (obradenKraj.ContainsKey(susjedni))
                        {
                            continue;
                        }
                        double privremenaUdaljenost = mapaVrijednostiOdKraja[trenutniOdKraja] + trenutniOdKraja.duljinauMetrima;
                        if (!mapaVrijednostiOdKraja.ContainsKey(susjedni) || privremenaUdaljenost < mapaVrijednostiOdKraja[susjedni])
                        {
                            mapaVrijednostiOdKraja[susjedni] = privremenaUdaljenost;
                            dosliIZKraj[susjedni] = trenutniOdKraja;

                            if (!mapHeapVrhKraj.ContainsKey(susjedni))
                            {
                                FibonacciHeapNode<Vrh, double> susjedniNodeKraj = new FibonacciHeapNode<Vrh, double>(susjedni, privremenaUdaljenost);
                                heapOdKraja.Insert(susjedniNodeKraj);
                                mapHeapVrhKraj[susjedni] = susjedniNodeKraj;
                            }
                            else
                            {
                                FibonacciHeapNode<Vrh, double> postojeciNodeKraj = mapHeapVrhKraj[susjedni];
                                heapOdKraja.DecreaseKey(postojeciNodeKraj, privremenaUdaljenost);
                            }
                        }
                    }
                }
            }
            if (zajednickiVrh != null)
            {
                DateTime vEnd = DateTime.Now;
                RutaSPP rBD = new RutaSPP();

                rBD.listaLinkova = new List<int>();
                rBD.udaljenostStvarna = 0;
                rBD.udaljenostIzračunata = najboljaVrijednost;
                rBD.opis = "BidirectionalDijkstra";
                rBD.udaljenostHeuristika = 0;
                rBD.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rBD.opis);
                Vrh trenutni = zajednickiVrh;
                rBD.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;
                while (dosliIZKraj.ContainsKey(trenutni))
                {
                    rBD.listaLinkova.Add(trenutni.RoadID);
                    trenutni = dosliIZKraj[trenutni];
                }
                rBD.listaLinkova.Add(kraj.RoadID);
                trenutni = zajednickiVrh;

                while (dosliIZPocetak.ContainsKey(trenutni))
                {
                    if (!rBD.listaLinkova.Contains(trenutni.RoadID))
                    {
                        rBD.listaLinkova.Insert(0, trenutni.RoadID);
                    }
                    trenutni = dosliIZPocetak[trenutni];
                }
                if (!rBD.listaLinkova.Contains(pocetak.RoadID))
                {
                    rBD.listaLinkova.Insert(0, pocetak.RoadID);
                }
                rBD.udaljenostStvarna = 0;
                double xp = 0, yp = 0, xk = 0, yk = 0;
                for (int i = 0; i < rBD.listaLinkova.Count; i++)
                {
                    Vrh tr = sviVrhovi[rBD.listaLinkova[i]];
                    if (tr != kraj)
                    {
                        rBD.udaljenostStvarna += tr.duljinauMetrima;
                    }
                    if (i == 0)
                    {
                        Vrh next = sviVrhovi[rBD.listaLinkova[i + 1]];
                        if (equalPoints(tr.pocetakX, trenutni.pocetakY, next.pocetakX, next.pocetakY) ||
                            equalPoints(tr.pocetakX, trenutni.pocetakY, next.krajX, next.krajY))
                        {
                            xp = tr.krajX;
                            yp = tr.krajY;
                            xk = tr.pocetakX;
                            yk = tr.pocetakY;
                        }
                        else
                        {
                            xp = tr.pocetakX;
                            yp = tr.pocetakY;
                            xk = tr.krajX;
                            yk = tr.krajY;
                        }
                        rBD.rutaNaKarti.Points.Add(new PointLatLng(yp, xp));
                        rBD.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                    }
                    else
                    {
                        if (equalPoints(tr.pocetakX, trenutni.pocetakY, xp, yp))
                        {
                            xk = tr.krajX;
                            yk = tr.krajY;
                        }
                        else
                        {
                            xk = tr.pocetakX;
                            yk = tr.pocetakY;
                        }
                        rBD.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                    }
                    xp = xk;
                    yp = yk;
                }
                rBD.rutaNaKarti.Stroke = new Pen(Color.Orange, 3);
                NacrtajRutu(rBD);
                return rBD;
            }
            return null;
        }

        private void btnBiDijkstra_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaBidirectionalDijkstra = BidirectionalDijkstra(pocetak, kraj);
                    if (rutaBidirectionalDijkstra != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaBidirectionalDijkstra.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool equalPoints(double x1, double y1, double x2, double y2)
        {
            if (Math.Abs(x1 - x2) < 0.000000000000001 && Math.Abs(y1 - y2) < 0.000000000000001)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public class RutaSPP
        {
            public List<int> listaLinkova;
            public double udaljenostStvarna;
            public double udaljenostIzračunata;
            public double udaljenostHeuristika;
            public string opis;
            public double vrijemeIzračuna;
            public GMapRoute rutaNaKarti;
            public RutaSPP(Dictionary<Vrh, Vrh> dosliIz, Vrh pocetak, Vrh krajnji, string opis, double vrijemeIzračuna, Pen olovka, double udaljenostIzracun, double udaljenostHeuristika = 0)
            {
                listaLinkova = new List<int>();
                udaljenostStvarna = 0;
                udaljenostIzračunata = udaljenostIzracun;
                this.opis = opis;
                this.udaljenostHeuristika = udaljenostHeuristika;
                Vrh trenutni = krajnji;
                this.vrijemeIzračuna = vrijemeIzračuna;

                rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + opis);
                while (dosliIz.ContainsKey(trenutni))
                {
                    if (trenutni != krajnji)
                    {
                        udaljenostStvarna += trenutni.duljinauMetrima;
                    }
                    listaLinkova.Insert(0, trenutni.RoadID);
                    rutaNaKarti.Points.Add(new PointLatLng(trenutni.pocetakY, trenutni.pocetakX));
                    rutaNaKarti.Points.Add(new PointLatLng(trenutni.krajY, trenutni.krajX));
                    trenutni = dosliIz[trenutni];
                }
                listaLinkova.Insert(0, pocetak.RoadID);
                udaljenostStvarna += pocetak.duljinauMetrima;
                rutaNaKarti.Points.Add(new PointLatLng(trenutni.pocetakY, trenutni.pocetakX));
                rutaNaKarti.Points.Add(new PointLatLng(trenutni.krajY, trenutni.krajX));
                rutaNaKarti.Stroke = olovka;
            }
            public RutaSPP()
            {

            }
            public string Detalji()
            {
                string s = "Udaljenost stvarna: " + udaljenostStvarna + "[m]\n";
                s += "Udaljenost izračunata: " + udaljenostIzračunata + "[m]\n";
                s += "Udaljenost heuristika: " + udaljenostHeuristika + "[m]\n";
                s += "Broj linkova: " + listaLinkova.Count + "\n";
                s += "Vrijeme izračuna: " + vrijemeIzračuna + "[ms]\n";
                s += "Opis: " + opis;
                return s;
            }


        }

        private void btnAStarBezFibonacci_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaAStarBezFib = AstarBezFibonaccija(pocetak, kraj);
                    if (rutaAStarBezFib != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaAStarBezFib.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private RutaSPP AstarBezFibonaccija(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            List<Vrh> zatvoreniSkup = new List<Vrh>();
            List<Vrh> otvoreniSkup = new List<Vrh>();
            Dictionary<Vrh, Vrh> dosliIZ = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> Gvrijednost = new Dictionary<Vrh, double>();
            Dictionary<Vrh, double> fVrijednost = new Dictionary<Vrh, double>();

            otvoreniSkup.Add(pocetak);
            Gvrijednost[pocetak] = 0;
            fVrijednost[pocetak] = heuristickaVrijednost(pocetak, kraj);

            while (otvoreniSkup.Count > 0)
            {
                Vrh trenutni = null;
                double najmanjaVrijednost = double.MaxValue;

                foreach (Vrh vrh in otvoreniSkup)
                {
                    double vrijednost = fVrijednost[vrh];
                    if (vrijednost < najmanjaVrijednost)
                    {
                        najmanjaVrijednost = vrijednost;
                        trenutni = vrh;
                    }
                }

                if (trenutni.Equals(kraj))
                {
                    DateTime vEnd = DateTime.Now;
                    RutaSPP rutaAStarBezFib = new RutaSPP();
                    rutaAStarBezFib.listaLinkova = new List<int>();
                    rutaAStarBezFib.udaljenostStvarna = 0;
                    rutaAStarBezFib.udaljenostIzračunata = Gvrijednost[trenutni];
                    rutaAStarBezFib.opis = "AStarBezFibonaccija";
                    rutaAStarBezFib.udaljenostHeuristika = Math.Round(heuristickaVrijednost(pocetak, kraj), 2);
                    rutaAStarBezFib.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaAStarBezFib.opis);
                    Vrh trenutniVrh = trenutni;
                    rutaAStarBezFib.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;

                    double xp = 0, yp = 0, xk = 0, yk = 0;
                    while (dosliIZ.ContainsKey(trenutniVrh))
                    {
                        rutaAStarBezFib.listaLinkova.Add(trenutniVrh.RoadID);
                        trenutniVrh = dosliIZ[trenutniVrh];
                    }
                    rutaAStarBezFib.listaLinkova.Add(pocetak.RoadID);

                    for (int i = 0; i < rutaAStarBezFib.listaLinkova.Count; i++)
                    {
                        Vrh tr = sviVrhovi[rutaAStarBezFib.listaLinkova[i]];
                        if (tr != kraj)
                        {
                            rutaAStarBezFib.udaljenostStvarna += tr.duljinauMetrima;
                        }
                        if (i == 0)
                        {
                            Vrh trenutni1 = sviVrhovi[rutaAStarBezFib.listaLinkova[i + 1]];
                            if (equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.pocetakX, trenutni1.pocetakY) ||
                                equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.krajX, trenutni1.krajY))
                            {
                                xp = tr.krajX;
                                yp = tr.krajY;
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            else
                            {
                                xp = tr.pocetakX;
                                yp = tr.pocetakY;
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            rutaAStarBezFib.rutaNaKarti.Points.Add(new PointLatLng(yp, xp));
                            rutaAStarBezFib.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        else
                        {
                            if (equalPoints(tr.pocetakX, tr.pocetakY, xp, yp))
                            {
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            else
                            {
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            rutaAStarBezFib.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        xp = xk;
                        yp = yk;
                    }
                    rutaAStarBezFib.rutaNaKarti.Stroke = new Pen(Color.Orange, 3);
                    NacrtajRutu(rutaAStarBezFib);
                    return rutaAStarBezFib;
                }

                otvoreniSkup.Remove(trenutni);
                zatvoreniSkup.Add(trenutni);

                foreach (int id in trenutni.susjedniLink)
                {
                    Vrh susjedni = sviVrhovi[id];
                    if (zatvoreniSkup.Contains(susjedni))
                    {
                        continue;
                    }
                    double privremenaGvrijednost = Gvrijednost[trenutni] + trenutni.duljinauMetrima;
                    if (!Gvrijednost.ContainsKey(susjedni) || privremenaGvrijednost < Gvrijednost[susjedni])
                    {
                        Gvrijednost[susjedni] = privremenaGvrijednost;
                        fVrijednost[susjedni] = Gvrijednost[susjedni] + heuristickaVrijednost(susjedni, kraj);
                        dosliIZ[susjedni] = trenutni;

                        if (!otvoreniSkup.Contains(susjedni))
                        {
                            otvoreniSkup.Add(susjedni);
                        }
                    }
                }
            }
            return null;
        }
        private void btnDijkstraBezFibonaccija_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP rutaDijkstraBezFib = DijkstraBezFib(pocetak, kraj);
                    if (rutaDijkstraBezFib != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + rutaDijkstraBezFib.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public RutaSPP DijkstraBezFib(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            List<Vrh> zatvoreniSkup = new List<Vrh>();
            List<Vrh> otvoreniSkup = new List<Vrh>();
            Dictionary<Vrh, Vrh> dosliIz = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> mapaVrijednosti = new Dictionary<Vrh, double>();

            otvoreniSkup.Add(pocetak);
            mapaVrijednosti[pocetak] = 0;

            while (otvoreniSkup.Count > 0)
            {
                Vrh trenutni = null;
                double najmanjaVrijednost = double.MaxValue;
                foreach (Vrh vrh in otvoreniSkup)
                {
                    double vrijednost = mapaVrijednosti[vrh];
                    if (vrijednost < najmanjaVrijednost)
                    {
                        najmanjaVrijednost = vrijednost;
                        trenutni = vrh;
                    }
                }

                if (trenutni.Equals(kraj))
                {
                    DateTime vEnd = DateTime.Now;
                    RutaSPP rutaDijkstraBezFib = new RutaSPP();
                    rutaDijkstraBezFib.listaLinkova = new List<int>();
                    rutaDijkstraBezFib.udaljenostStvarna = 0;
                    rutaDijkstraBezFib.udaljenostIzračunata = mapaVrijednosti[trenutni];
                    rutaDijkstraBezFib.opis = "DijkstraBezFibonaccija";
                    rutaDijkstraBezFib.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rutaDijkstraBezFib.opis);
                    Vrh trenutniVrh = trenutni;
                    rutaDijkstraBezFib.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;

                    double xp = 0, yp = 0, xk = 0, yk = 0;
                    while (dosliIz.ContainsKey(trenutniVrh))
                    {
                        rutaDijkstraBezFib.listaLinkova.Add(trenutniVrh.RoadID);
                        trenutniVrh = dosliIz[trenutniVrh];
                    }
                    rutaDijkstraBezFib.listaLinkova.Add(pocetak.RoadID);

                    for (int i = 0; i < rutaDijkstraBezFib.listaLinkova.Count; i++)
                    {
                        Vrh tr = sviVrhovi[rutaDijkstraBezFib.listaLinkova[i]];
                        if (tr != kraj)
                        {
                            rutaDijkstraBezFib.udaljenostStvarna += tr.duljinauMetrima;
                        }
                        if (i == 0)
                        {
                            Vrh trenutni1 = sviVrhovi[rutaDijkstraBezFib.listaLinkova[i + 1]];
                            if (equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.pocetakX, trenutni1.pocetakY) ||
                                equalPoints(tr.pocetakX, tr.pocetakY, trenutni1.krajX, trenutni1.krajY))
                            {
                                xp = tr.krajX;
                                yp = tr.krajY;
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            else
                            {
                                xp = tr.pocetakX;
                                yp = tr.pocetakY;
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            rutaDijkstraBezFib.rutaNaKarti.Points.Add(new PointLatLng(yp, xp));
                            rutaDijkstraBezFib.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        else
                        {
                            if (equalPoints(tr.pocetakX, tr.pocetakY, xp, yp))
                            {
                                xk = tr.krajX;
                                yk = tr.krajY;
                            }
                            else
                            {
                                xk = tr.pocetakX;
                                yk = tr.pocetakY;
                            }
                            rutaDijkstraBezFib.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                        }
                        xp = xk;
                        yp = yk;
                    }
                    rutaDijkstraBezFib.rutaNaKarti.Stroke = new Pen(Color.Red, 3);
                    NacrtajRutu(rutaDijkstraBezFib);
                    return rutaDijkstraBezFib;
                }

                otvoreniSkup.Remove(trenutni);
                zatvoreniSkup.Add(trenutni);

                foreach (int id in trenutni.susjedniLink)
                {
                    Vrh susjedni = sviVrhovi[id];
                    if (zatvoreniSkup.Contains(susjedni))
                    {
                        continue;
                    }

                    double privremenaUdaljenost = mapaVrijednosti[trenutni] + trenutni.duljinauMetrima;
                    if (!mapaVrijednosti.ContainsKey(susjedni) || privremenaUdaljenost < mapaVrijednosti[susjedni])
                    {
                        mapaVrijednosti[susjedni] = privremenaUdaljenost;
                        dosliIz[susjedni] = trenutni;

                        if (!otvoreniSkup.Contains(susjedni))
                        {
                            otvoreniSkup.Add(susjedni);
                        }
                    }
                }
            }
            return null;
        }

        private void btnBiAStar_Click(object sender, EventArgs e)
        {
            try
            {
                if (AlgoritamSpreman && brojmarkera == 2)
                {

                    RutaSPP BiAStar = BidirectionalAStar(pocetak, kraj);
                    if (BiAStar != null)
                    {
                        MessageBox.Show("Ruta je pronađena! Detalji\n " + BiAStar.Detalji());

                    }
                    else
                    {
                        MessageBox.Show("Ruta nije pronađena");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private RutaSPP BidirectionalAStar(Vrh pocetak, Vrh kraj)
        {
            DateTime vStart = DateTime.Now;
            Dictionary<Vrh, Vrh> dosliIZPocetak = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, Vrh> obradeniPocetak = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> mapaVrijednostiOdPocetka = new Dictionary<Vrh, double>();
            FibonacciHeap<Vrh, double> heapOdPocetka = new FibonacciHeap<Vrh, double>(double.MaxValue);
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> mapHeapVrhPocetak = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();

            Dictionary<Vrh, Vrh> obradenKraj = new Dictionary<Vrh, Vrh>();
            Dictionary<Vrh, double> mapaVrijednostiOdKraja = new Dictionary<Vrh, double>();
            FibonacciHeap<Vrh, double> heapOdKraja = new FibonacciHeap<Vrh, double>(double.MaxValue);
            Dictionary<Vrh, FibonacciHeapNode<Vrh, double>> mapHeapVrhKraj = new Dictionary<Vrh, FibonacciHeapNode<Vrh, double>>();

            Dictionary<Vrh, Vrh> dosliIZKraj = new Dictionary<Vrh, Vrh>();



            FibonacciHeapNode<Vrh, double> pocetniHeap = new FibonacciHeapNode<Vrh, double>(pocetak, heuristickaVrijednost(pocetak, kraj));
            heapOdPocetka.Insert(pocetniHeap);
            mapHeapVrhPocetak[pocetak] = pocetniHeap;
            mapaVrijednostiOdPocetka[pocetak] = 0;

            FibonacciHeapNode<Vrh, double> krajHeap = new FibonacciHeapNode<Vrh, double>(kraj, heuristickaVrijednost(kraj, pocetak));
            mapHeapVrhKraj[kraj] = krajHeap;
            heapOdKraja.Insert(krajHeap);
            mapaVrijednostiOdKraja[kraj] = 0;

            Vrh zajednickiVrh = null;
            double najboljaVrijednost = double.MaxValue;

            while (!heapOdKraja.IsEmpty() && !heapOdPocetka.IsEmpty())
            {
                if (!heapOdPocetka.IsEmpty())
                {
                    FibonacciHeapNode<Vrh, double> trenutniNodeOdPocetka = heapOdPocetka.RemoveMin();
                    obradeniPocetak[trenutniNodeOdPocetka.Data] = trenutniNodeOdPocetka.Data;
                    Vrh trenutniOdPocetka = trenutniNodeOdPocetka.Data;

                    if (mapaVrijednostiOdKraja.ContainsKey(trenutniOdPocetka))
                    {
                        double ukupnaVrijednost = mapaVrijednostiOdPocetka[trenutniOdPocetka] + mapaVrijednostiOdKraja[trenutniOdPocetka];
                        if (ukupnaVrijednost < najboljaVrijednost)
                        {
                            najboljaVrijednost = ukupnaVrijednost;
                            zajednickiVrh = trenutniOdPocetka;
                        }
                        if (obradeniPocetak.ContainsKey(trenutniOdPocetka) && obradenKraj.ContainsKey(trenutniOdPocetka))
                        {
                            break;
                        }
                    }

                    foreach (int id in trenutniOdPocetka.susjedniLink)
                    {
                        Vrh susjedni = sviVrhovi[id];
                        if (obradeniPocetak.ContainsKey(susjedni))
                        {
                            continue;
                        }
                        double privremenaUdaljenost = mapaVrijednostiOdPocetka[trenutniOdPocetka] + trenutniOdPocetka.duljinauMetrima;
                        double heuristickaVrijednost1 = privremenaUdaljenost + heuristickaVrijednost(susjedni, kraj);
                        if (!mapaVrijednostiOdPocetka.ContainsKey(susjedni) || privremenaUdaljenost < mapaVrijednostiOdPocetka[susjedni])
                        {
                            mapaVrijednostiOdPocetka[susjedni] = privremenaUdaljenost;
                            dosliIZPocetak[susjedni] = trenutniOdPocetka;

                            if (!mapHeapVrhPocetak.ContainsKey(susjedni))
                            {
                                FibonacciHeapNode<Vrh, double> susjedniNodePocetak = new FibonacciHeapNode<Vrh, double>(susjedni, heuristickaVrijednost1);
                                heapOdPocetka.Insert(susjedniNodePocetak);
                                mapHeapVrhPocetak[susjedni] = susjedniNodePocetak;
                            }
                            else
                            {
                                FibonacciHeapNode<Vrh, double> postojeciNodePocetak = mapHeapVrhPocetak[susjedni];
                                heapOdPocetka.DecreaseKey(postojeciNodePocetak, heuristickaVrijednost1);
                            }
                        }
                    }
                }

                if (!heapOdKraja.IsEmpty())
                {
                    FibonacciHeapNode<Vrh, double> trenutniNodeOdKraja = heapOdKraja.RemoveMin();
                    Vrh trenutniOdKraja = trenutniNodeOdKraja.Data;
                    obradenKraj[trenutniOdKraja] = trenutniOdKraja;

                    if (mapaVrijednostiOdPocetka.ContainsKey(trenutniOdKraja))
                    {
                        double ukupnaVrijednost = mapaVrijednostiOdPocetka[trenutniOdKraja] + mapaVrijednostiOdKraja[trenutniOdKraja];
                        if (ukupnaVrijednost < najboljaVrijednost)
                        {
                            najboljaVrijednost = ukupnaVrijednost;
                            zajednickiVrh = trenutniOdKraja;
                        }
                        if (obradeniPocetak.ContainsKey(trenutniOdKraja) && obradenKraj.ContainsKey(trenutniOdKraja))
                        {
                            break;
                        }
                    }

                    foreach (int id in trenutniOdKraja.linkoviPrijeZaBiDijkstru)
                    {
                        Vrh susjedni = sviVrhovi[id];
                        if (obradenKraj.ContainsKey(susjedni))
                        {
                            continue;
                        }
                        double privremenaUdaljenost = mapaVrijednostiOdKraja[trenutniOdKraja] + trenutniOdKraja.duljinauMetrima;
                        double heuristickaVrijednost1 = privremenaUdaljenost + heuristickaVrijednost(susjedni, pocetak);
                        if (!mapaVrijednostiOdKraja.ContainsKey(susjedni) || privremenaUdaljenost < mapaVrijednostiOdKraja[susjedni])
                        {
                            mapaVrijednostiOdKraja[susjedni] = privremenaUdaljenost;
                            dosliIZKraj[susjedni] = trenutniOdKraja;

                            if (!mapHeapVrhKraj.ContainsKey(susjedni))
                            {
                                FibonacciHeapNode<Vrh, double> susjedniNodeKraj = new FibonacciHeapNode<Vrh, double>(susjedni, heuristickaVrijednost1);
                                heapOdKraja.Insert(susjedniNodeKraj);
                                mapHeapVrhKraj[susjedni] = susjedniNodeKraj;
                            }
                            else
                            {
                                FibonacciHeapNode<Vrh, double> postojeciNodeKraj = mapHeapVrhKraj[susjedni];
                                heapOdKraja.DecreaseKey(postojeciNodeKraj, heuristickaVrijednost1);
                            }
                        }
                    }
                }
            }

            if (zajednickiVrh != null)
            {
                DateTime vEnd = DateTime.Now;
                RutaSPP rBA = new RutaSPP();

                rBA.listaLinkova = new List<int>();
                rBA.udaljenostStvarna = 0;
                rBA.udaljenostIzračunata = najboljaVrijednost;
                rBA.opis = "BidirectionalAStar";
                rBA.udaljenostHeuristika = heuristickaVrijednost(pocetak, kraj);
                rBA.rutaNaKarti = new GMapRoute(new List<PointLatLng>(), "ruta" + rBA.opis);
                Vrh trenutni = zajednickiVrh;
                rBA.vrijemeIzračuna = (vEnd - vStart).TotalMilliseconds;

                while (dosliIZKraj.ContainsKey(trenutni))
                {
                    rBA.listaLinkova.Add(trenutni.RoadID);
                    trenutni = dosliIZKraj[trenutni];
                }
                rBA.listaLinkova.Add(kraj.RoadID);
                trenutni = zajednickiVrh;

                while (dosliIZPocetak.ContainsKey(trenutni))
                {
                    if (!rBA.listaLinkova.Contains(trenutni.RoadID))
                    {
                        rBA.listaLinkova.Insert(0, trenutni.RoadID);
                    }
                    trenutni = dosliIZPocetak[trenutni];
                }
                if (!rBA.listaLinkova.Contains(pocetak.RoadID))
                {
                    rBA.listaLinkova.Insert(0, pocetak.RoadID);
                }
                rBA.udaljenostStvarna = 0;
                double xp = 0, yp = 0, xk = 0, yk = 0;
                for (int i = 0; i < rBA.listaLinkova.Count; i++)
                {
                    Vrh tr = sviVrhovi[rBA.listaLinkova[i]];
                    if (tr != kraj)
                    {
                        rBA.udaljenostStvarna += tr.duljinauMetrima;
                    }
                    if (i == 0)
                    {
                        Vrh next = sviVrhovi[rBA.listaLinkova[i + 1]];
                        if (equalPoints(tr.pocetakX, trenutni.pocetakY, next.pocetakX, next.pocetakY) ||
                            equalPoints(tr.pocetakX, trenutni.pocetakY, next.krajX, next.krajY))
                        {
                            xp = tr.krajX;
                            yp = tr.krajY;
                            xk = tr.pocetakX;
                            yk = tr.pocetakY;
                        }
                        else
                        {
                            xp = tr.pocetakX;
                            yp = tr.pocetakY;
                            xk = tr.krajX;
                            yk = tr.krajY;
                        }
                        rBA.rutaNaKarti.Points.Add(new PointLatLng(yp, xp));
                        rBA.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                    }
                    else
                    {
                        if (equalPoints(tr.pocetakX, trenutni.pocetakY, xp, yp))
                        {
                            xk = tr.krajX;
                            yk = tr.krajY;
                        }
                        else
                        {
                            xk = tr.pocetakX;
                            yk = tr.pocetakY;
                        }
                        rBA.rutaNaKarti.Points.Add(new PointLatLng(yk, xk));
                    }
                    xp = xk;
                    yp = yk;
                }
                rBA.rutaNaKarti.Stroke = new Pen(Color.Magenta, 3);
                NacrtajRutu(rBA);
                return rBA;

            }
            return null;
        }

        private void btnSimulacija_Click(object sender, EventArgs e)
        {
            Simulacija();
        }
        private void Simulacija()
        {
            try
            {


                Random rnd = new Random();
                List<Vrh> listvrhova = sviVrhovi.Values.ToList();
                StreamWriter pisanje = new StreamWriter("SIMULACIJA.txt");
                int brojac10 = 0;
                int brojac1050 = 0;
                int brojac50100 = 0;
                int brojac100 = 0;
                double[] rezultat = new double[7];
                pisanje.WriteLine("KATEGORIJA; A*; A* bez fibonaccija; Dijkstra; Dijkstra bez Fib; BiDijkstra; BiA*; GBFS");
                while (brojac10 < 100)
                {
                    int indeksPoc = rnd.Next(listvrhova.Count);
                    Vrh vrhPocetak = listvrhova[indeksPoc];

                    int indeksKraj = rnd.Next(listvrhova.Count);
                    Vrh vrhKraj = listvrhova[indeksKraj];

                    double udaljenost = airalDistHaversine(vrhPocetak.pocetakX, vrhPocetak.pocetakY, vrhKraj.pocetakX, vrhKraj.pocetakY);

                    if (vrhPocetak == vrhKraj || (udaljenost < 100 || udaljenost > 10000))
                    {
                        continue;
                    }
                    else if (udaljenost > 0 && udaljenost <= 10000 && brojac10 < 100)
                    {
                        rezultat[0] = IzracunVremena(() => AstarAlgoritam(vrhPocetak, vrhKraj));
                        rezultat[1] = IzracunVremena(() => AstarBezFibonaccija(vrhPocetak, vrhKraj));
                        rezultat[2] = IzracunVremena(() => Dijkstra(vrhPocetak, vrhKraj));
                        rezultat[3] = IzracunVremena(() => DijkstraBezFib(vrhPocetak, vrhKraj));
                        rezultat[4] = IzracunVremena(() => BidirectionalDijkstra(vrhPocetak, vrhKraj));
                        rezultat[5] = IzracunVremena(() => BidirectionalAStar(vrhPocetak, vrhKraj));
                        rezultat[6] = IzracunVremena(() => GreedyBestFirstSearch(vrhPocetak, vrhKraj));
                        brojac10++;
                        pisanje.Write("<10;");
                        for (int i = 0; i < rezultat.Length; i++)
                        {
                            if (i > 0)
                            {
                                pisanje.Write(";");
                            }
                            pisanje.Write(rezultat[i]);
                        }
                        pisanje.WriteLine();

                    }
                }
                while (brojac1050 < 100)
                {
                    int indeksPoc = rnd.Next(listvrhova.Count);
                    Vrh vrhPocetak = listvrhova[indeksPoc];

                    int indeksKraj = rnd.Next(listvrhova.Count);
                    Vrh vrhKraj = listvrhova[indeksKraj];

                    double udaljenost = airalDistHaversine(vrhPocetak.pocetakX, vrhPocetak.pocetakY, vrhKraj.pocetakX, vrhKraj.pocetakY);

                    if (vrhPocetak == vrhKraj || udaljenost < 10000 || udaljenost > 50000)
                    {
                        continue;
                    }
                    else if (udaljenost > 10000 && udaljenost <= 50000 && brojac1050 < 100)
                    {
                        rezultat[0] = IzracunVremena(() => AstarAlgoritam(vrhPocetak, vrhKraj));
                        rezultat[1] = IzracunVremena(() => AstarBezFibonaccija(vrhPocetak, vrhKraj));
                        rezultat[2] = IzracunVremena(() => Dijkstra(vrhPocetak, vrhKraj));
                        rezultat[3] = IzracunVremena(() => DijkstraBezFib(vrhPocetak, vrhKraj));
                        rezultat[4] = IzracunVremena(() => BidirectionalDijkstra(vrhPocetak, vrhKraj));
                        rezultat[5] = IzracunVremena(() => BidirectionalAStar(vrhPocetak, vrhKraj));
                        rezultat[6] = IzracunVremena(() => GreedyBestFirstSearch(vrhPocetak, vrhKraj));
                        brojac1050++;
                        pisanje.Write(">10&&<50;");
                        for (int i = 0; i < rezultat.Length; i++)
                        {
                            if (i > 0)
                            {
                                pisanje.Write(";");
                            }
                            pisanje.Write(rezultat[i]);
                        }
                        pisanje.WriteLine();

                    }
                }

                while (brojac50100 < 100)
                {
                    int indeksPoc = rnd.Next(listvrhova.Count);
                    Vrh vrhPocetak = listvrhova[indeksPoc];

                    int indeksKraj = rnd.Next(listvrhova.Count);
                    Vrh vrhKraj = listvrhova[indeksKraj];

                    double udaljenost = airalDistHaversine(vrhPocetak.pocetakX, vrhPocetak.pocetakY, vrhKraj.pocetakX, vrhKraj.pocetakY);

                    if (vrhPocetak == vrhKraj || udaljenost < 50000 || udaljenost > 100000)
                    {
                        continue;
                    }
                    else if (udaljenost > 50000 && udaljenost <= 100000 && brojac50100 < 100)
                    {
                        rezultat[0] = IzracunVremena(() => AstarAlgoritam(vrhPocetak, vrhKraj));
                        rezultat[1] = IzracunVremena(() => AstarBezFibonaccija(vrhPocetak, vrhKraj));
                        rezultat[2] = IzracunVremena(() => Dijkstra(vrhPocetak, vrhKraj));
                        rezultat[3] = IzracunVremena(() => DijkstraBezFib(vrhPocetak, vrhKraj));
                        rezultat[4] = IzracunVremena(() => BidirectionalDijkstra(vrhPocetak, vrhKraj));
                        rezultat[5] = IzracunVremena(() => BidirectionalAStar(vrhPocetak, vrhKraj));
                        rezultat[6] = IzracunVremena(() => GreedyBestFirstSearch(vrhPocetak, vrhKraj));
                        brojac50100++;
                        pisanje.Write(">50&&<100;");
                        for (int i = 0; i < rezultat.Length; i++)
                        {
                            if (i > 0)
                            {
                                pisanje.Write(";");
                            }
                            pisanje.Write(rezultat[i]);
                        }
                        pisanje.WriteLine();


                    }
                }
                while (brojac100 < 100)
                {
                    int indeksPoc = rnd.Next(listvrhova.Count);
                    Vrh vrhPocetak = listvrhova[indeksPoc];

                    int indeksKraj = rnd.Next(listvrhova.Count);
                    Vrh vrhKraj = listvrhova[indeksKraj];

                    double udaljenost = airalDistHaversine(vrhPocetak.pocetakX, vrhPocetak.pocetakY, vrhKraj.pocetakX, vrhKraj.pocetakY);

                    if (vrhPocetak == vrhKraj || udaljenost < 100000)
                    {
                        continue;
                    }
                    else if (udaljenost > 100000 && brojac100 < 100)
                    {
                        rezultat[0] = IzracunVremena(() => AstarAlgoritam(vrhPocetak, vrhKraj));
                        rezultat[1] = IzracunVremena(() => AstarBezFibonaccija(vrhPocetak, vrhKraj));
                        rezultat[2] = IzracunVremena(() => Dijkstra(vrhPocetak, vrhKraj));
                        rezultat[3] = IzracunVremena(() => DijkstraBezFib(vrhPocetak, vrhKraj));
                        rezultat[4] = IzracunVremena(() => BidirectionalDijkstra(vrhPocetak, vrhKraj));
                        rezultat[5] = IzracunVremena(() => BidirectionalAStar(vrhPocetak, vrhKraj));
                        rezultat[6] = IzracunVremena(() => GreedyBestFirstSearch(vrhPocetak, vrhKraj));
                        brojac100++;

                        pisanje.Write(">100;");
                        for (int i = 0; i < rezultat.Length; i++)
                        {
                            if (i > 0)
                            {
                                pisanje.Write(";");
                            }
                            pisanje.Write(rezultat[i]);
                        }
                        pisanje.WriteLine();
                    }

                }

                pisanje.Flush();
                pisanje.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static double IzracunVremena(Func<RutaSPP> algoritam)
        {
            Stopwatch sw = Stopwatch.StartNew();
            RutaSPP rezultat = algoritam();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

    }
}
