using Microsoft.Data.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public class Chromosomes
    {
        public double Fitness  = double.NaN;
        public List<int> Unit = new List<int>();

          
        public Chromosomes(List<int> _Unit)
        {
            this.Unit = _Unit;
        }
    }
    public class GA
    {
        List<Chromosomes> ChromosomesALL = new List<Chromosomes>();
        private int GALength = 0;
        private int GACount = 0;
        double CrossoverRate = 0.8;
        double MutationRate = 0.3;
        double ElitistRate = 0.2;
        int StopGeneration = 100;
        int GenerationLimit = 3000;
        public int MaxGeneration = 0;
        public List<int> MaxUnit;
        public double MaxFetness = 0;
        int CurrentGeneration = 0;

        public GA(int _GALength, int _GACount, double _CrossoverRate, double _MutationRate)
        {
            this.GACount = _GACount;
            this.GALength = _GALength;
            this.CrossoverRate = _CrossoverRate;
            this.MutationRate = _MutationRate;
        }


        //產生初始基因
        public void InitPopulation()
        {
            Random random = new Random();
            for (int i = 0; i < this.GACount; i++)
            {
                List<int> Chromosome1 = new List<int>();
                for (int j = 0; j < this.GALength; j++)
                {
                    int randomNumber = random.Next(0, 101);
                    Chromosome1.Add(randomNumber);
                }
                ChromosomesALL.Add(new Chromosomes(Chromosome1));
            }
        }

      

        //取得適應值Fitness
        public void Evaluation(Func<List<int>, DataFrame ,double > GetFitness1,  DataFrame df)
        {
            for (int i = 0; i < GenerationLimit; i++)
            {
                CurrentGeneration = i;
                int old = 0;
                foreach (Chromosomes item in ChromosomesALL)
                {
                    if (item == null)
                    {
                        string ss = "";
                    }
                    if (double.IsNaN(item.Fitness))
                        item.Fitness = GetFitness1(item.Unit, df);
                    else
                        old++;


                    if (item.Fitness > MaxFetness)
                    {
                        MaxFetness = item.Fitness;
                        GetFitness1(item.Unit, df);
                        MaxUnit = new List<int> (item.Unit);                          ;
                        MaxGeneration = CurrentGeneration;
                    }
                }
                if (CurrentGeneration > MaxGeneration + StopGeneration)
                {
                    break;
                }
                Selection();
                Crossover(); 
            }
            
        }

        //輪盤法選擇留下來的基因
        public void Selection()
        {
            double fitness = 0;
            double fitnessMin = 0;
            double fitnessMax = 0;

            ChromosomesALL.Sort((x, y) => -y.Fitness.CompareTo(x.Fitness));
            int ElitistCount = (int)(this.ElitistRate * GACount);
            for (int i = 0; i < ChromosomesALL.Count- ElitistCount; i++)
            {
                if (ChromosomesALL[i].Fitness >= 0)
                    fitness += ChromosomesALL[i].Fitness;
            }
            //foreach (Chromosomes item in ChromosomesALL)
            //{
            //    if (item.Fitness >= 0)
            //        fitness += item.Fitness;
            //}

            double[] WheelRange = new double[GACount];
            double[] WheelS = new double[GACount];
            double[] WheelE = new double[GACount];
            List<Chromosomes> ChromosomesNew = new List<Chromosomes>();

            double Start = 0;
            for (int i = 0; i < ChromosomesALL.Count - ElitistCount; i++)
            {

                double range = (ChromosomesALL[i].Fitness) / fitness;

                if (ChromosomesALL[i].Fitness < 0)
                    range = 0.001;
                else if (ChromosomesALL[i].Fitness == 0)
                    range = 0.1;
                WheelRange[i] = range;
                WheelS[i] = Start;
                WheelE[i] = Start + WheelRange[i];
                Start += WheelRange[i];
            }
            ArrayList lList = new ArrayList();
            Random random = new Random();
            while (ChromosomesNew.Count< ChromosomesALL.Count - ElitistCount)
            {

                double randomNumber = random.NextDouble();
                for (int j = 0; j < GACount; j++)
                {
                    if (randomNumber > WheelS[j] && randomNumber <= WheelE[j])
                    {
                        ChromosomesNew.Add(ChromosomesALL[j]);
                        lList.Add(randomNumber);
                        break;
                    }
                }
            }
           
            if (ChromosomesNew.Count != GACount- ElitistCount)
            {
                string sx = "";
            }
            else
            {
                for (int i = 0; i < GACount- ElitistCount; i++)
                {
                    ChromosomesALL[i] = ChromosomesNew[i];
                }
                //ChromosomesALL = ChromosomesNew;
            }
        }

        //交配 突變
        public void Crossover()
        {
            List<Chromosomes> ChromosomesNew = new List<Chromosomes>();
            Random random = new Random();
            while (ChromosomesNew.Count < GACount * CrossoverRate)
            {
                int randomIndex = random.Next(ChromosomesALL.Count);
                ChromosomesNew.Add(ChromosomesALL[randomIndex]);
                ChromosomesALL.RemoveAt(randomIndex);
            }
            //ChromosomesNew.Sort((x, y) => y.Fitness.CompareTo(x.Fitness));

            for (int i = 0; i < ChromosomesNew.Count-1; i = i + 2)
            {
                ChromosomesALL.AddRange(Crossover1(ChromosomesNew[i], ChromosomesNew[i + 1]));
            }
        }

        //突變
        public void Mutation(ref List<int> Unit)
        {
            Random random = new Random();
            double randomNumber = random.NextDouble();
            if(randomNumber< MutationRate)
            {
                int i=random.Next(0, GALength - 1);
                Unit[i] = random.Next(0, 101);
            } 
        }

        public List<Chromosomes> Crossover1(Chromosomes Chromosomes1, Chromosomes Chromosomes2)
        {
            List<int> list1 = Chromosomes1.Unit;
            List<int> list2 = Chromosomes2.Unit;

            Random random = new Random();
            int splitIndex1 = random.Next(1, list1.Count - 1);
            int splitIndex2 = random.Next(splitIndex1 + 1, list1.Count);

            List<int> segment1 = list1.GetRange(0, splitIndex1);
            List<int> segment2 = list1.GetRange(splitIndex1, splitIndex2 - splitIndex1);
            List<int> segment3 = list1.GetRange(splitIndex2, list1.Count - splitIndex2);

            list1.Clear();
            list1.AddRange(segment2);
            list1.AddRange(segment1);
            list1.AddRange(segment3);

            List<int> correspondingSegment = list2.GetRange(splitIndex1, splitIndex2 - splitIndex1);
            list2.RemoveRange(splitIndex1, splitIndex2 - splitIndex1);
            list2.InsertRange(splitIndex1, correspondingSegment);

            Chromosomes1.Unit = list1;
            Chromosomes1.Fitness = double.NaN;
            Mutation(ref Chromosomes1.Unit);
            Chromosomes2.Unit = list2;
            Chromosomes2.Fitness = double.NaN;
            Mutation(ref Chromosomes2.Unit);
            List<Chromosomes> newChromosomes = new List<Chromosomes>();
            newChromosomes.Add(Chromosomes1);
            newChromosomes.Add(Chromosomes2);
            return newChromosomes;

        }
    }
}
