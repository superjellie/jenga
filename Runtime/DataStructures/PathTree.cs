using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {


    public class PathTree<T> {

        public T value;

        Dictionary<string, PathTree<T>> children = new();

        public PathTree(T value = default(T)) {
            this.value = value;
        }

        public IEnumerable<(string, T)> Walk(bool emitKeys = false) {
            foreach (var (key, child) in children) {
                yield return (key, child.value);

                foreach (var (path, x) in child.Walk(emitKeys))
                    yield return (emitKeys ? path : $"{key}/{path}", x);
            }
        }

        public bool TryFindValue(Predicate<T> pred, out T value) {
            if (TryFindSubtree(pred, out var tree)) {
                value = tree.value;
                return true;
            }

            value = default(T);
            return false;
        }


        public bool TryFindSubtree(Predicate<T> pred, out PathTree<T> node) {
            node = null;

            if (pred(value)) {
                node = this;
                return true;
            }

            foreach (var (key, child) in children)
                if (child.TryFindSubtree(pred, out node)) 
                    return true;
                
            return false;
        }


        public bool TryGetValue(string path, out T value) {
            if (TryGetSubtree(path, out var node)) {
                value = node.value;
                return true;
            }

            value = default(T);
            return false;
        }

        public bool TryGetSubtree(string path, out PathTree<T> node) {
            node = null;

            if (string.IsNullOrEmpty(path)) {
                node = this;
                return true;
            }

            var i = path.IndexOf('/');

            var pathKey = i >= 0 ? path.Substring(0, i) : path;
            var pathRest = i >= 0 ? path.Substring(i + 1) : "";

            if (children.TryGetValue(pathKey, out var child)) 
                return child.TryGetSubtree(pathRest, out node);
                
            return false;
        }

        public void Add(string path, T value) {

            if (string.IsNullOrEmpty(path)) {
                this.value = value;
                return;
            }

            var i = path.IndexOf('/');

            var pathKey = i >= 0 ? path.Substring(0, i) : path;
            var pathRest = i >= 0 ? path.Substring(i + 1) : "";

            if (!children.TryGetValue(pathKey, out var child)) 
                children.Add(pathKey, child = new(value));
            
            child.Add(pathRest, value);
        }

        public void Clear() {
            children.Clear();
            value = default(T);
        }
    }
    
}
