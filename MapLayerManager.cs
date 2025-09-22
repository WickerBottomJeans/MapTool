using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool {

    internal class MapLayerManager {
        private Dictionary<string, MapLayer> layersDict = new Dictionary<string, MapLayer>();

        public event EventHandler LayersDictChanged;

        protected virtual void OnLayersDictChanged() {
            LayersDictChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddLayer(MapLayer layer) {
            if (layer != null && !layersDict.ContainsKey(layer.Name)) {
                layersDict[layer.Name] = layer;
                OnLayersDictChanged();
            }
        }

        public void AddLayers(List<MapLayer> layers) {
            foreach (var layer in layers) {
                if (layer != null && !layersDict.ContainsKey(layer.Name)) {
                    layersDict[layer.Name] = layer;
                }
            }
            OnLayersDictChanged();
        }

        public void RemoveLayer(string name) {
            if (layersDict.ContainsKey(name)) {
                layersDict.Remove(name);
                OnLayersDictChanged();
            }
        }

        public MapLayer GetLayer(string name) {
            layersDict.TryGetValue(name, out var layer);
            return layer;
        }

        public IEnumerable<MapLayer> GetAllLayers() => layersDict.Values;

        public void AddLayer(string name, int pixelWidth, int pixelHeight) {
            if (!string.IsNullOrWhiteSpace(name) && pixelWidth > 0 && pixelHeight > 0 && !layersDict.ContainsKey(name)) {
                MapLayer newLayer = new MapLayer(name, pixelHeight, pixelWidth, true);
                layersDict[name] = newLayer;
                OnLayersDictChanged();
            }
        }
    }
}