
import { Header, Item, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/strores/store";
import { observer } from "mobx-react-lite";
import ActivityListItem from "./ActivityListItem";
import { Fragment } from "react";



export default observer(function ActivityList() {
    const { activityStore } = useStore();
    const { groupedActivities } = activityStore;
    
    
    return (
        <>
            {
                groupedActivities.map(([group, activities]) => {
                    return (<Fragment key={group}>
                        <Header sub color="teal">
                            {group}
                        </Header>
                        {activities.map((activity, index) => (
                            <ActivityListItem key={ index } activity={activity}/>
                        ))}
                    </Fragment>)
                })
            }
        </>
    );
});