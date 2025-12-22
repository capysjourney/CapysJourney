/*

Class: FirebaseManager.cs
==============================================
Last update: 2018-05-20  (by Dikra)
==============================================


* MIT LICENSE
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
* the following conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using Firebase;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{

    private static FirebaseManager _instance;
    private static readonly object _lock = new();

    private bool _isFirebaseInitialized = false;
    public bool IsFirebaseInitialized => _isFirebaseInitialized;

    #region INSTANCE_LOGICS
    public static FirebaseManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    FirebaseManager[] managers = FindObjectsByType<FirebaseManager>(
                        FindObjectsSortMode.None);

                    _instance = (managers.Length > 0) ? managers[0] : null;

                    if (managers.Length > 1)
                    {
                        Debug.LogError("[Firebase Manager] Something went really wrong " +
                            " - there should never be more than 1 Firebase Manager!" +
                            " Reopening the scene might fix it.");

                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new();
                        _instance = singleton.AddComponent<FirebaseManager>();
                        singleton.name = "Firebase Manager [Singleton]";

                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                        Debug.Log("[Firebase Manager] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }

    public void SetFirebaseInitialized(bool isInitialized)
    {
        _isFirebaseInitialized = isInitialized;
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            if (Instance != this)
                Destroy(this);
        }
    }
    #endregion

}