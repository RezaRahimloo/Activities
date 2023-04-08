import { Grid } from "semantic-ui-react";
import ActivityList from "./ActivityList";
import { useStore } from "../../../app/strores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import ActivityFilters from "./ActivityFilters";



export default observer(function ActivityDashboard() {
    const { activityStore } = useStore();
    const { loadActivities, activityRegistery } = activityStore;

    useEffect(() => {
        if (activityRegistery.size <= 0) {
            loadActivities();
        }
    }, [activityRegistery.size, loadActivities]);

  
  
  
    if (activityStore.loadingInitial) return (<LoadingComponent />)
    
    return (
        <Grid>
            <Grid.Column width='10' >
                <ActivityList/>
            </Grid.Column>
            <Grid.Column width='6' >
                <ActivityFilters/>
            </Grid.Column>
        </Grid>
    )
});