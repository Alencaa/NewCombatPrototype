using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Class Effect đã được cập nhật để chứa một danh sách prefab
[System.Serializable]
public class Effect
{
    public string name;
    public List<GameObject> prefabs;
    [Tooltip("Số lượng hiệu ứng được tạo sẵn cho MỖI BIẾN THỂ trong danh sách trên.")]
    public int poolSize = 5;
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private List<Effect> effectCategories;

    // Pool chính giờ đây sẽ dùng chính Prefab làm key để truy cập
    private Dictionary<GameObject, Queue<GameObject>> pool;
    // Dictionary phụ để liên kết tên danh mục và danh sách các prefab biến thể
    private Dictionary<string, List<GameObject>> effectVariants;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // --- LOGIC KHỞI TẠO POOL MỚI ---
        pool = new Dictionary<GameObject, Queue<GameObject>>();
        effectVariants = new Dictionary<string, List<GameObject>>();

        foreach (Effect category in effectCategories)
        {
            // Thêm danh sách các biến thể vào dictionary phụ
            effectVariants.Add(category.name, category.prefabs);

            // Lặp qua từng prefab biến thể trong danh mục
            foreach (GameObject variantPrefab in category.prefabs)
            {
                // Tạo một hàng đợi cho loại prefab biến thể này
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < category.poolSize; i++)
                {
                    GameObject obj = Instantiate(variantPrefab);
                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }
                // Thêm hàng đợi vào pool chính với key là chính prefab đó
                pool.Add(variantPrefab, objectQueue);
            }
        }
    }

    /// <summary>
    /// Kích hoạt một hiệu ứng từ Pool.
    /// </summary>
    /// <param name="categoryName">Tên của danh mục hiệu ứng.</param>
    /// <param name="position">Vị trí để hiệu ứng xuất hiện.</param>
    /// <param name="rotation">Hướng xoay của hiệu ứng.</param>
    /// <param name="parent">Nếu được cung cấp, hiệu ứng sẽ "dính" vào và di chuyển cùng với parent này.</param>
    public void PlayEffect(string categoryName, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        // 1. Kiểm tra xem có danh mục hiệu ứng này không
        if (!effectVariants.ContainsKey(categoryName))
        {
            Debug.LogWarning("Effect category with name " + categoryName + " doesn't exist.");
            return;
        }

        // 2. Lấy danh sách các prefab biến thể từ danh mục đó
        List<GameObject> variants = effectVariants[categoryName];
        if (variants == null || variants.Count == 0)
        {
            Debug.LogWarning("Effect category " + categoryName + " has no prefabs assigned.");
            return;
        }

        // 3. Chọn ngẫu nhiên một prefab từ danh sách biến thể
        GameObject chosenPrefab = variants[Random.Range(0, variants.Count)];

        // 4. Lấy hàng đợi tương ứng với prefab đã chọn từ pool chính
        if (!pool.ContainsKey(chosenPrefab))
        {
            Debug.LogError("Pool is missing an entry for prefab: " + chosenPrefab.name);
            return;
        }
        Queue<GameObject> objectQueue = pool[chosenPrefab];

        // 5. Lấy một object từ kho
        if (objectQueue.Count == 0)
        {
            // Tùy chọn: Nếu hết object trong pool, có thể tạo thêm một cái mới
            Debug.LogWarning("Pool for " + chosenPrefab.name + " is empty. Consider increasing pool size.");
            GameObject newObj = Instantiate(chosenPrefab);
            objectQueue.Enqueue(newObj);
        }

        GameObject effectToPlay = objectQueue.Dequeue();

        // --- LOGIC MỚI ĐỂ XỬ LÝ PARENT ---
        if (parent != null)
        {
            // Nếu có parent, gán hiệu ứng làm con và đặt vị trí local
            effectToPlay.transform.SetParent(parent);
            effectToPlay.transform.position = position; // Có thể bạn muốn dùng localPosition tùy trường hợp
            effectToPlay.transform.rotation = rotation;
        }
        else
        {
            // Nếu không có parent, đảm bảo nó không còn là con của object nào cả
            // và đặt vị trí trong không gian thế giới (world space)
            effectToPlay.transform.SetParent(null);
            effectToPlay.transform.position = position;
            effectToPlay.transform.rotation = rotation;
        }


        effectToPlay.SetActive(true);
        objectQueue.Enqueue(effectToPlay);
    }
}