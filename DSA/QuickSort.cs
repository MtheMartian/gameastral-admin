namespace GameStarBackend.DSA
{
    public class QuickSort
    {
        static private List<int> SortHelper(List<int> arr, int low, int high)
        {
            if(low >= high)
            {
                return arr;
            }

            int pivotIdx = Partition(arr, low, high);
            SortHelper(arr, low, pivotIdx - 1);
            SortHelper(arr, pivotIdx + 1, high);

            return arr;
        }

        static private int Partition(List<int> arr, int low, int high)
        {
            int idx = low - 1;
            int pivot = arr[high];

            for(int i = low; i < high; i++)
            {
                if (arr[i] >= pivot)
                {
                    idx++;
                    int tmp = arr[i];
                    arr[i] = arr[idx];
                    arr[idx] = tmp;
                }
            }

            idx++;
            arr[high] = arr[idx];
            arr[idx] = pivot;

            return idx;
        }

        static public List<int> Sort(List<int> arr)
        {
            return SortHelper(arr, 0, arr.Count - 1);
        }
    }
}
