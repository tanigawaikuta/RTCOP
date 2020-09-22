#ifndef __RTCOP_GENERATED_RTCOPAPPINITIALIZER__
#define __RTCOP_GENERATED_RTCOPAPPINITIALIZER__

#include "RTCOP/Core/Initializer.h"

namespace RTCOP {

namespace Core {
class RTCOPManager;
class LayerdObjectInitializer;
class LayerActivater;
}

namespace Generated {
class RTCOPAppInitializer : public Core::Initializer
{
public:
	explicit RTCOPAppInitializer();
	virtual ~RTCOPAppInitializer();
	virtual Core::RTCOPManager* InitializeRTCOPManager() override;
	virtual Core::LayerdObjectInitializer* InitializeLayerdObjectInitializer(Core::RTCOPManager* manager) override;
	virtual Core::LayerActivater* InitializeLayerActivater(Core::RTCOPManager* manager) override;
	virtual void RegisterLayers(Core::RTCOPManager* manager) override;
	virtual void FirstLayerActivation(Core::LayerActivater* activater) override;
};

} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_RTCOPAPPINITIALIZER__
