using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePreview : MonoBehaviour
{
    [Header("������ ����")]
    public Material previewMaterial;
    public Color validColor = new Color(0, 1, 0, 0.5f);   // �ʷϻ� ������
    public Color invalidColor = new Color(1, 0, 0, 0.5f); // ������ ������
    public LayerMask previewLayer;

    public GameObject previewCamera;
    private GameObject currentPreview;
    private GridManager gridManager;
    private Camera playerCamera;
    private SimplePlacer simplePlacer;

    private Dictionary<string, GameObject> previews = new Dictionary<string, GameObject>();

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerCamera = Camera.main;
        simplePlacer = FindObjectOfType<SimplePlacer>();

        simplePlacer.OnChangeFurniture += ChangePreview;

        CreatePreviewMaterial();
        InitializePreviews();
    }

    

    void CreatePreviewMaterial()
    {
        if (previewMaterial == null)
        {
            previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // ���� ������ ����
            previewMaterial.SetFloat("_Surface", 1); // 0=Opaque, 1=Transparent
            previewMaterial.SetFloat("_Blend", 0);   // Alpha blending
            previewMaterial.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetInt("_ZWrite", 0);

            previewMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            previewMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }

    void InitializePreviews()
    {
        foreach(var preview in FurnitureManager.Instance.furnitureDatas)
        {
            previews[preview.id] = CreatePreviewObject(preview.furniturePrefab.gameObject);
        }
    }

    void Update()
    {
        UpdatePreview();
    }

    private void OnDisable()
    {
        HidePreview();
    }

    public void ChangePreview(string id)
    {
        HidePreview();

        if (previews.ContainsKey(id))
            currentPreview = previews[id];
        else
            currentPreview = null;
    }

    void UpdatePreview()
    {
        if (simplePlacer == null || simplePlacer.currentFurniture == null) return;

        // ��ġ ��尡 �ƴϸ� ������ �����
        if (simplePlacer.currentMode == SimplePlacer.PlaceMode.Remove)
        {
            HidePreview();
            return;
        }

        Vector2Int gridPos = simplePlacer.targetPosition;

        if (gridManager.IsValidGridPosition(gridPos.x, gridPos.y))
        {
            Vector3 worldPos = gridManager.GridToWorldPosition(gridPos.x, gridPos.y);
            bool canPlace = CanPlaceAt(
                new Vector2Int(gridPos.x, gridPos.y),
                simplePlacer.currentFurniture.furniturePrefab.Size,
                GetDirectionAngle(simplePlacer.currentRotation)
                );

            ShowPreview(worldPos, canPlace);
        }
        else
        {
            HidePreview();
        }
    }

    bool CanPlaceAt(Vector2Int start, Vector2Int size, int rotation)
    {
        bool canPlace = true;

        // ��ġ ������ �������� Ȯ��
        if(!gridManager.AreAllTilesValid(start, size, rotation)) canPlace = false;

        // �ش� ��ġ�� �̹� �ǹ��� �ִ��� Ȯ��
        if (gridManager.HasAnyBuildingInRange(start, size, rotation)) canPlace = false;     

        return canPlace; // ��ġ ����
    }

    void ShowPreview(Vector3 position, bool canPlace)
    {
        if (currentPreview != null)
        {
            currentPreview.SetActive(true);
            currentPreview.transform.position = position;

            float angle = GetDirectionAngle(simplePlacer.currentRotation);
            currentPreview.transform.rotation = Quaternion.Euler(0, angle, 0);

            // ���� ����
            Color targetColor = canPlace ? validColor : invalidColor;
            SetPreviewColor(targetColor);
        }

        if (previewCamera != null)
        {
            previewCamera.SetActive(true);
        }
    }

    void HidePreview()
    {
        if (currentPreview != null)
        {
            currentPreview.SetActive(false);
        }
        if (previewCamera != null)
        {
            previewCamera.SetActive(false);
        }
    }

    GameObject CreatePreviewObject(GameObject previewObject)
    {
        GameObject preview = null;

        if (previewObject != null)
        {
            preview = Instantiate(previewObject);
            preview.name = $"Preview_{previewObject.name}";

            // �ݶ��̴� ���� (�浹 ����)
            Collider[] colliders = preview.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            // ��� �������� ������ ��Ƽ���� ����
            Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer.CompareTag("Arrow"))
                    continue;                                   // Arrow�� �ǳʶٱ�

                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }
                renderer.materials = materials;
            }

            // ���̾� ����
            preview.layer = previewLayer;

            preview.SetActive(false);

            return preview;
        }

        return null;
    }


    void SetPreviewColor(Color color)
    {
        if (currentPreview == null) return;

        previewMaterial.color = color;

        Renderer[] renderers = currentPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.CompareTag("Arrow"))
                continue;                                   // Arrow�� �ǳʶٱ�

            renderer.material.color = color;
        }
    }

    int GetDirectionAngle(int direction)
    {
        int[] angles = { 0, 90, 180, 270 };
        return angles[direction % 4];
    }

    void OnDestroy()
    {
        if (currentPreview != null)
        {
            DestroyImmediate(currentPreview);
        }

        simplePlacer.OnChangeFurniture += ChangePreview;
    }
}
