using services.saves;

namespace services {
public interface PlayerPrefsLoadListener {
    public void onPrefsLoaded(PlayerPrefsData prefs);
}
}